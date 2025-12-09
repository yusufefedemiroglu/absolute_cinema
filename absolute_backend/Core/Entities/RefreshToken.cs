public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }

    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    // Foreign key
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}