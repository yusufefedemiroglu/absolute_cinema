namespace Core.Events.Orders;

public record PaymentFailedEvent(Guid CorrelationId, Guid ProductId, decimal Amount, Guid OrderId, string Reason);
