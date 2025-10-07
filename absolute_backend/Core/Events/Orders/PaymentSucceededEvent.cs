namespace Core.Events.Orders;

public record PaymentSucceededEvent(Guid CorrelationId, int OrderId);
