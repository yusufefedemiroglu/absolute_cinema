using API.Security;
using Application.Abstractions;
using Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private const string RefreshCookieName = "refreshToken";

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/auth/register
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(result);
    }

    // POST api/auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        var refreshToken = HttpContext.Items["RefreshToken"] as string ?? "";
        Response.Cookies.Append(
    RefreshCookieName,
    refreshToken,
    CookieOptionsFactory.RefreshToken(result.RefreshTokenExpiresAtUtc)
);
        return Ok(result);
    }

    // POST api/auth/refresh
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName] ?? throw new UnauthorizedAccessException
        ("Refresh token cookie is missing.");

        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new UnauthorizedAccessException("Access token is missing.");
        }

        var result = await _authService.RefreshTokenAsync(accessToken, refreshToken, cancellationToken);

        var newRefreshToken = HttpContext.Items["RefreshToken"] as string ?? "";
        if (!string.IsNullOrEmpty(newRefreshToken))
        {
            Response.Cookies.Append(
        RefreshCookieName,
        newRefreshToken,
        CookieOptionsFactory.RefreshToken(result.RefreshTokenExpiresAtUtc)
    );
        }
        return Ok(result);
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return NoContent();

        await _authService.RevokeRefreshTokenAsync(
            refreshToken,
            cancellationToken
        );

        Response.Cookies.Delete("refreshToken");

        return NoContent();
    }
}