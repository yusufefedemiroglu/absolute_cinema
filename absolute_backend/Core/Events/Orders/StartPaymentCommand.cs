namespace Core.Events.Orders;

public record StartPaymentCommand(Guid CorrelationId, Guid OrderId, Guid ProductId, decimal Amount);