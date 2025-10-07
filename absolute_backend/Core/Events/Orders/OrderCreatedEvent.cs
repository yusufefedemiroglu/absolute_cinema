namespace Core.Events.Orders;

public record OrderCreatedEvent(Guid CorrelationId, int OrderId, int ProductId, decimal Amount);
