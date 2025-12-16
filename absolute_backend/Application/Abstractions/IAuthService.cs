using Application.DTOs.Auth;

namespace Application.Abstractions;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken = default
        );
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}