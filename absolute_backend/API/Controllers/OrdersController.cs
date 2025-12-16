using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // 🔹 Create Order
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var result = await _orderService.CreateOrderAsync(dto.ProductId, dto.Amount);
        return Ok(result);
    }

    // 🔹 One specific order status
    [HttpGet("status/{correlationId:guid}")]
    public async Task<IActionResult> GetStatus(Guid correlationId)
    {
        var order = await _orderService.GetStatusAsync(correlationId);
        if (order == null) return NotFound(new { message = "Order not found" });
        return Ok(order);
    }

}

public record CreateOrderDto(Guid ProductId, decimal Amount);