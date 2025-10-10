namespace Core.Events.Orders;

public record PaymentFailedEvent(Guid CorrelationId, Guid OrderId, string Reason);
