namespace Core.Events.Orders;

public record OrderCreatedEvent(Guid CorrelationId, Guid OrderId, Guid ProductId, decimal Amount);
