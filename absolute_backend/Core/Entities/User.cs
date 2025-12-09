using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    public string PasswordSalt { get; set; } = "";

    public string Roles { get; set; } = "User";

    public bool IsActive { get; set; } = true;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // Additional Properties
    public string FullName { get; set; } = "";
    public ICollection<Watchlist> Watchlist { get; set; } = new List<Watchlist>();
    public ICollection<History> History { get; set; } = new List<History>();
}
