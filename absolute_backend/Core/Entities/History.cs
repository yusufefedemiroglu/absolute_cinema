public class History
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int TitleId { get; set; }
    public Title Title { get; set; } = null!;

    public DateTime LastVisitedAt { get; set; } = DateTime.UtcNow;
}
