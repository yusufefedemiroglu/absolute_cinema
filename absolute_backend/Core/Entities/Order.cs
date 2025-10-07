namespace Core.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserEmail { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";

    // relationships
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
