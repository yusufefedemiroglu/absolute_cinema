public sealed class AuthResponseDto
{
    public string AccessToken { get; set; } = "";
    // public string RefreshToken { get; set; } = "";
    public DateTime AccessTokenExpiresAtUtc { get; set; }
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
}
internal sealed class AuthInternalResult
{
    public AuthResponseDto Public { get; set; } = null!;
    public string RefreshToken { get; set; } = "";
}