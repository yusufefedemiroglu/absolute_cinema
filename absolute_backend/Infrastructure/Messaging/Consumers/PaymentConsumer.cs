using Core.Events.Orders;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class PaymentConsumer : IConsumer<StartPaymentCommand>
{
    private readonly IPublishEndpoint _publish;

    public PaymentConsumer(IPublishEndpoint publish)
    {
        _publish = publish;
    }

    public async Task Consume(ConsumeContext<StartPaymentCommand> context)
    {
        var order = context.Message;
        Console.WriteLine($"💳 Processing payment for Order {order.OrderId}");
        await Task.Delay(2000); // Simulate payment processing delay
        // %80 chance of success of simulation
        var rnd = new Random();
        if (rnd.NextDouble() > 0.2)
        {
            await _publish.Publish(new PaymentSucceededEvent(order.CorrelationId, order.ProductId, order.Amount, order.OrderId));
        }
        else
        {
            await _publish.Publish(new PaymentFailedEvent(order.CorrelationId, order.ProductId, order.Amount, order.OrderId, "Insufficient funds"));
        }
    }
}
