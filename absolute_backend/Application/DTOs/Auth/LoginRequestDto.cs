namespace Application.DTOs.Auth;

public sealed class LoginRequestDto
{
    public string UserNameOrEmail { get; set; } = "";
    public string Password { get; set; } = "";
}