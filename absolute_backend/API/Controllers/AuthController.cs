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
        return Ok(result);
    }

    // POST api/auth/refresh
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(result);
    }

    // POST api/auth/revoke
    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(
        [FromBody] RevokeTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
        return NoContent();
    }
}