using Core.Events.Orders;
using MassTransit;
using Serilog; 

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


        // ---------------------------
        // INITIAL
        // ---------------------------

        Initially(
            When(OrderCreated)
                .ThenAsync(async ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.ProductId = ctx.Message.ProductId;
                    ctx.Saga.Amount = ctx.Message.Amount;

                    Log.Information(
                        "📦 OrderCreated received | CorrelationId: {CorrelationId}, OrderId: {OrderId}, ProductId: {ProductId}, Amount: {Amount}",
                        ctx.Saga.CorrelationId, ctx.Saga.OrderId, ctx.Saga.ProductId, ctx.Saga.Amount
                    );

                    await ctx.Publish(new StartPaymentCommand(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.OrderId,
                        ctx.Saga.ProductId,
                        ctx.Saga.Amount
                    ));

                    Log.Information(
                        "💳 StartPaymentCommand published for OrderId: {OrderId}",
                        ctx.Saga.OrderId
                    );
                })
                .TransitionTo(Processing)
        );


        // ---------------------------
        // PROCESSING STATE
        // ---------------------------

        During(Processing,

            When(PaymentSucceeded)
                .Then(ctx =>
                {
                    Log.Information(
                        "✅ PaymentSucceeded received | OrderId: {OrderId}, CorrelationId: {CorrelationId}",
                        ctx.Saga.OrderId, ctx.Saga.CorrelationId
                    );
                })
                .Finalize(),

            When(PaymentFailed)
                .Then(ctx =>
                {
                    Log.Warning(
                        "❌ PaymentFailed received | OrderId: {OrderId}, Reason: {Reason}, CorrelationId: {CorrelationId}",
                        ctx.Saga.OrderId, ctx.Message.Reason, ctx.Saga.CorrelationId
                    );
                })
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}