namespace Core.Entities;

public class OrderHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }

    public decimal Amount { get; set; }
    public string Status { get; set; } = default!; // "Succeeded" / "Failed"
    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime CompletedAt { get; set; }
}