public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FullName { get; set; } = "";

    public ICollection<Watchlist> Watchlist { get; set; } = new List<Watchlist>();
    public ICollection<History> History { get; set; } = new List<History>();
}
