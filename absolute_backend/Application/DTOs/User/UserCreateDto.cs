public class UserCreateDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    // Opsiyonel – register sırasında alacaksan
    public string FullName { get; set; } = string.Empty;

    public string Roles { get; set; } = "User";
}