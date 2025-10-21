using Core.Events.Orders;
using MassTransit;

namespace Infrastructure.Messaging.Sagas;

public class OrderSaga : MassTransitStateMachine<OrderState>
{
    public State? Processing { get; private set; }
    public Event<OrderCreatedEvent>? OrderCreated { get; private set; }
    public Event<PaymentSucceededEvent>? PaymentSucceeded { get; private set; }
    public Event<PaymentFailedEvent>? PaymentFailed { get; private set; }

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreated, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => PaymentSucceeded, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => PaymentFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Initially(
            When(OrderCreated)
                .ThenAsync(async ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.ProductId = ctx.Message.ProductId;
                    ctx.Saga.Amount = ctx.Message.Amount;
                    Console.WriteLine($"🛒 Order {ctx.Saga.OrderId} created → waiting for payment...");

                    await ctx.Publish(new StartPaymentCommand(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.OrderId,
                        ctx.Saga.ProductId,
                        ctx.Saga.Amount));
                })
                .TransitionTo(Processing)
        );

        During(Processing,
            When(PaymentSucceeded)
                .Then(ctx =>
                {
                    Console.WriteLine($"✅ Payment success for Order {ctx.Saga.OrderId}");
                })
                .Finalize(),

            When(PaymentFailed)
                .Then(ctx =>
                {
                    Console.WriteLine($"❌ Payment failed for Order {ctx.Saga.OrderId}: {ctx.Message.Reason}");
                })
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
