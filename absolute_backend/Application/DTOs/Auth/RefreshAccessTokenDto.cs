namespace Application.DTOs.Auth;

public sealed class RefreshAccessTokenDto
{
    public string AccessToken { get; set; } = null!;
}