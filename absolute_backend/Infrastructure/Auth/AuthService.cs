using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions;
using Application.DTOs.Auth;
using Core.Entities;
using Infrastructure.Auth;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Auth;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IDatabase _redis;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext dbContext,
        ITokenService tokenService,
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _redis = connectionMultiplexer.GetDatabase();
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    // REGISTER
    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        // Email veya kullanıcı adı alınmış mı?
        var exists = await _dbContext.Users
            .AnyAsync(u => u.Email == request.Email || u.Username == request.UserName, cancellationToken);

        if (exists)
            throw new InvalidOperationException("User with this email or username already exists.");

        CreatePasswordHash(request.Password, out var hash, out var salt);

        var user = new User
        {
            Email = request.Email,
            Username = request.UserName,
            FullName = request.FullName,
            PasswordHash = hash,
            PasswordSalt = salt,
            Roles = "User",
            IsActive = true
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var authResponse = await GenerateAndPersistTokensAsync(user, cancellationToken);
        _logger.LogInformation("User {UserId} registered.", user.Id);

        return authResponse;
    }

    // LOGIN
    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(
                u => u.Username == request.UserNameOrEmail || u.Email == request.UserNameOrEmail,
                cancellationToken);

        if (user is null)
            throw new InvalidOperationException("Invalid credentials.");

        if (!user.IsActive)
            throw new InvalidOperationException("User is disabled.");

        if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new InvalidOperationException("Invalid credentials.");

        var authResponse = await GenerateAndPersistTokensAsync(user, cancellationToken);
        _logger.LogInformation("User {UserId} logged in.", user.Id);

        return authResponse;
    }

    // REFRESH
    public async Task<AuthResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto request,
        CancellationToken cancellationToken = default)
    {
        // 1) Expired access token'dan userId çek
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken)
                       ?? throw new InvalidOperationException("Invalid access token.");

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
                          ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new InvalidOperationException("Invalid token payload.");

        // 2) Redis üzerinden hızlı check
        var redisKey = GetRefreshKey(request.RefreshToken);
        var redisUserId = await _redis.StringGetAsync(redisKey);
        if (redisUserId.IsNullOrEmpty)
        {
            // Redis'te yoksa DB'ye bakacağız (cache miss)
            _logger.LogDebug("Refresh token not found in Redis, falling back to DB.");
        }
        else if (redisUserId != userId.ToString())
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        // 3) DB üzerinden refresh token doğrula
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        var tokenEntity = user.RefreshTokens.SingleOrDefault(rt => rt.Token == request.RefreshToken);

        if (tokenEntity is null || tokenEntity.IsExpired || tokenEntity.IsRevoked)
            throw new InvalidOperationException("Invalid refresh token.");

        // Eski refresh token'ı revoke et
        tokenEntity.RevokedAtUtc = DateTime.UtcNow;

        // Redis'ten temizle
        await _redis.KeyDeleteAsync(redisKey);

        var authResponse = await GenerateAndPersistTokensAsync(user, cancellationToken);
        _logger.LogInformation("User {UserId} refreshed token.", user.Id);

        return authResponse;
    }

    // LOGOUT / REVOKE
    public async Task RevokeRefreshTokenAsync(
        RevokeTokenRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (token is null)
            return;

        token.RevokedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _redis.KeyDeleteAsync(GetRefreshKey(request.RefreshToken));
        _logger.LogInformation("Refresh token revoked for user {UserId}.", token.UserId);
    }

    // Ortak token üretim + DB + Redis kaydı
    private async Task<AuthResponseDto> GenerateAndPersistTokensAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, accessExpires) = _tokenService.GenerateAccessToken(user);
        var (refreshToken, refreshExpires) = _tokenService.GenerateRefreshToken(user);

        var refreshEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAtUtc = refreshExpires,
            UserId = user.Id
        };

        _dbContext.RefreshTokens.Add(refreshEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Redis cache: refresh:{token} -> userId
        var ttl = refreshExpires - DateTime.UtcNow;
        await _redis.StringSetAsync(GetRefreshKey(refreshToken), user.Id.ToString(), ttl);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshTokenExpiresAtUtc = refreshExpires
        };
    }

    private static string GetRefreshKey(string token) => $"refresh:{token}";

    // PASSWORD HASHING

    private static void CreatePasswordHash(string password, out string hash, out string salt)
    {
        using var hmac = new HMACSHA512();
        var saltBytes = hmac.Key;
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        salt = Convert.ToBase64String(saltBytes);
        hash = Convert.ToBase64String(hashBytes);
    }

    private static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        using var hmac = new HMACSHA512(saltBytes);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computedHashString = Convert.ToBase64String(computedHash);
        return computedHashString == storedHash;
    }
}