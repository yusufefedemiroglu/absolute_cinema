using Core.Events.Orders;
using Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderService
{
    private readonly IPublishEndpoint _publish;
    private readonly AppDbContext _db;

    private readonly ILogger<OrderService> _logger;

    public OrderService(IPublishEndpoint publish, AppDbContext db, ILogger<OrderService> logger)
    {
        _publish = publish;
        _db = db;
        _logger = logger;
    }

    // 🔹 Create New Order
    public async Task<object> CreateOrderAsync(Guid productId, decimal amount)
    {
        var correlationId = Guid.NewGuid();

        var orderCreated = new OrderCreatedEvent(
            CorrelationId: correlationId,
            OrderId: Guid.NewGuid(),
            ProductId: productId,
            Amount: amount
        );

        await _publish.Publish(orderCreated);
        Console.WriteLine($"📦 OrderCreatedEvent published → CorrelationId: {correlationId}");
        _logger.LogInformation("OrderCreatedEvent published → CorrelationId: {CorrelationId}", correlationId);

        return new
        {
            CorrelationId = correlationId,
            orderCreated.OrderId,
            orderCreated.ProductId,
            orderCreated.Amount
        };
    }

    // 🔹 One Specific Order
    public async Task<object?> GetStatusAsync(Guid correlationId)
    {
        var order = await _db.OrderStates
            .Where(o => o.CorrelationId == correlationId)
            .Select(o => new
            {
                o.CorrelationId,
                o.OrderId,
                o.ProductId,
                o.Amount,
                o.CurrentState,
                o.CreatedAt
            })
            .FirstOrDefaultAsync();

        return order;
    }

    // 🔹 All orders
    public async Task<List<object>> GetAllAsync()
    {
        var orders = await _db.OrderStates
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.CorrelationId,
                o.OrderId,
                o.ProductId,
                o.Amount,
                o.CurrentState,
                o.CreatedAt
            })
            .ToListAsync();

        return orders.Cast<object>().ToList();
    }
}