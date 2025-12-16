using Microsoft.AspNetCore.Http;

namespace API.Security;

public static class CookieOptionsFactory
{
    public static CookieOptions RefreshToken(DateTime expiresAtUtc)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to true in production
            SameSite = SameSiteMode.Strict,
            Expires = expiresAtUtc,
            Path = "/api/auth/refresh"
        };
    }
}