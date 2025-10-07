namespace Core.Events.Orders;

public record PaymentFailedEvent(Guid CorrelationId, int OrderId, string Reason);
