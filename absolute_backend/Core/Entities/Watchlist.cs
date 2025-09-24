public class Watchlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int TitleId { get; set; }
    public Title Title { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
