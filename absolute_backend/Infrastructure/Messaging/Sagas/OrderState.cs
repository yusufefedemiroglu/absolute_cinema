using System.ComponentModel.DataAnnotations;
using MassTransit;

namespace Infrastructure.Messaging.Sagas;

public class OrderState : SagaStateMachineInstance
{
    [Key]
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
