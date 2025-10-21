namespace Core.Events.Orders;

public record PaymentSucceededEvent(Guid CorrelationId, Guid ProductId, decimal Amount, Guid OrderId);
