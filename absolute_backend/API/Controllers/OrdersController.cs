using Core.Events.Orders;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publish;

        public OrdersController(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        // 🎯 Saga’yı başlatan endpoint
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var correlationId = Guid.NewGuid(); // Saga'nın takip ID'si

            var orderCreatedEvent = new OrderCreatedEvent(
                CorrelationId: correlationId,
                OrderId: Guid.NewGuid(),
                ProductId: dto.ProductId,
                Amount: dto.Amount
            );

            await _publish.Publish(orderCreatedEvent);

            Console.WriteLine($"📦 OrderCreatedEvent published → CorrelationId: {correlationId}");

            return Ok(new
            {
                Message = "Order event published successfully!",
                CorrelationId = correlationId,
                OrderId = orderCreatedEvent.OrderId,
                ProductId = dto.ProductId,
                Amount = dto.Amount
            });
        }
    }

    public record CreateOrderDto(Guid ProductId, decimal Amount);
}