using Core.Entities;
using Core.Events.Orders;
using Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Messaging.Consumers;
//payment is gonna fixed later
public class OrderHistoryConsumer :
    IConsumer<PaymentSucceededEvent>,
    IConsumer<PaymentFailedEvent>
{
    private readonly AppDbContext _db;

    public OrderHistoryConsumer(AppDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var msg = context.Message;

        var history = new OrderHistory
        {
            CorrelationId = msg.CorrelationId,
            OrderId = msg.OrderId,
            ProductId = msg.ProductId,
            Amount = msg.Amount,
            Status = "Succeeded",
            Reason = null,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        await _db.OrderHistories.AddAsync(history);
        await _db.SaveChangesAsync();

        Console.WriteLine($"🟢 Audit → Order {msg.OrderId} succeeded, logged to history.");
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var msg = context.Message;

        var history = new OrderHistory
        {
            CorrelationId = msg.CorrelationId,
            OrderId = msg.OrderId,
            ProductId = Guid.Empty,
            Amount = 0,
            Status = "Failed",
            Reason = msg.Reason,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        await _db.OrderHistories.AddAsync(history);
        await _db.SaveChangesAsync();

        Console.WriteLine($"🔴 Audit → Order {msg.OrderId} failed: {msg.Reason}");
    }
}