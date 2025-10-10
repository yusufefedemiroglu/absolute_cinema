namespace Core.Events.Orders;

public record PaymentSucceededEvent(Guid CorrelationId, Guid OrderId);
