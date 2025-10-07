using Core.Events.Orders;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class PaymentConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IPublishEndpoint _publish;

    public PaymentConsumer(IPublishEndpoint publish)
    {
        _publish = publish;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        Console.WriteLine($"💳 Processing payment for Order {order.OrderId}");

        // %80 chance of success of simulation
        var rnd = new Random();
        if (rnd.NextDouble() > 0.2)
        {
            await _publish.Publish(new PaymentSucceededEvent(order.CorrelationId, order.OrderId));
        }
        else
        {
            await _publish.Publish(new PaymentFailedEvent(order.CorrelationId, order.OrderId, "Insufficient funds"));
        }
    }
}
