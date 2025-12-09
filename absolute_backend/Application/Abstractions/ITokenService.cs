using Core.Entities;
using System.Security.Claims;

namespace Application.Abstractions;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) GenerateAccessToken(User user);
    (string token, DateTime expiresAtUtc) GenerateRefreshToken(User user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
}