namespace Application.DTOs.Auth;

public sealed class RegisterRequestDto
{
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Password { get; set; } = "";
}