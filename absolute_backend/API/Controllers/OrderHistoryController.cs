using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderHistoryController : ControllerBase
{
    private readonly OrderHistoryService _service;

    public OrderHistoryController(OrderHistoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var history = await _service.GetAllAsync();
        return Ok(history);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var record = await _service.GetByOrderIdAsync(orderId);
        if (record == null) return NotFound();
        return Ok(record);
    }
}