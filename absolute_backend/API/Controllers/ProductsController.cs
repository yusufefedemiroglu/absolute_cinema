using Application.DTOs.Product;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductsController> _logger;


    public ProductsController(ProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }


    [HttpGet]
    [ProducesResponseType(typeof(List<ProductReadDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllReadAsync();
        return Ok(products);

    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Product Get by ID called — Guid: {TitleId},", id);

        var product = await _productService.GetReadByIdAsync(id);
        if (product == null)
            return NotFound(new { Message = "Product not found." });

        _logger.LogInformation("Product Get by ID worked — Guid: {TitleId},", id);

        return Ok(product);
    }


    [HttpGet("by-title/{titleId:int}")]
    [ProducesResponseType(typeof(List<ProductReadDto>), 200)]
    public async Task<IActionResult> GetByTitleId(int titleId)
    {
        var products = await _productService.GetByTitleIdAsync(titleId);
        return Ok(products);
    }


    [HttpPost("create/{titleId:int}")]
    [ProducesResponseType(typeof(ProductReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create(int titleId, [FromBody] ProductCreateDto dto)
    {
        var id = await _productService.CreateAsync(titleId, dto);

        var created = await _productService.GetReadByIdAsync(id);

        return CreatedAtAction(nameof(GetById), new { id }, created);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
    {
        var success = await _productService.UpdateAsync(id, dto);
        if (!success)
            return NotFound(new { Message = "Product not found." });

        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { Message = "Product not found." });

        return NoContent();
    }
}