using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions;
using Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly byte[] _key;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _key = Encoding.UTF8.GetBytes(_options.SecretKey);
    }

    public (string token, DateTime expiresAtUtc) GenerateAccessToken(User user)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            // Standard claims
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),

            // Roles for now single role as string, can be extended to multiple roles if needed
            new(ClaimTypes.Role, user.Roles),
            new(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)

        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(_key),
            SecurityAlgorithms.HmacSha256
        );

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return (token, expires);
    }

    public (string token, DateTime expiresAtUtc) GenerateRefreshToken(User user)
    {
        // User parameter is just for signature, can be used to link token to user if needed
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var token = Convert.ToBase64String(randomNumber);
        var expires = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

        return (token, expires);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_key),
            // here is the key part:
            // We want to parse the expired access token in the refresh flow
            // close the lifetime validation
            ValidateLifetime = false,
            ClockSkew = TimeSpan.FromMinutes(5) // --- ADDED ---
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(
                accessToken,
                tokenValidationParameters,
                out var securityToken
            );

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}