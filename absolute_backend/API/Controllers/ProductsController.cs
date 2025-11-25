using Application.Caching.Attributes;
using Application.DTOs.Product;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    // GET ALL
    [Cached(60)]
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductReadDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllReadAsync();
        return Ok(products);
    }

    // GET BY ID
    [Cached(30)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {

        var p = await _productService.GetReadByIdAsync(id);
        if (p == null)
            return NotFound(new { Message = "Product not found." });

        return Ok(p);
    }

    // GET BY TITLE ID
    [Cached(45)]
    [HttpGet("by-title/{titleId:int}")]
    [ProducesResponseType(typeof(List<ProductReadDto>), 200)]
    public async Task<IActionResult> GetByTitleId(int titleId)
    {
        var products = await _productService.GetByTitleIdAsync(titleId);
        return Ok(products);
    }

    // CREATE
    [InvalidateCache("api/products")]
    [HttpPost("create/{titleId:int}")]
    [ProducesResponseType(typeof(ProductReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create(int titleId, [FromBody] ProductCreateDto dto)
    {
        var id = await _productService.CreateAsync(titleId, dto);
        var created = await _productService.GetReadByIdAsync(id);

        return CreatedAtAction(nameof(GetById), new { id }, created);
    }

    // UPDATE
    [HttpPut("{id:guid}")]
    [InvalidateCache("api/products")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
    {
        var success = await _productService.UpdateAsync(id, dto);
        if (!success)
            return NotFound(new { Message = "Product not found." });

        return NoContent();
    }

    // DELETE
    [HttpDelete("{id:guid}")]
    [InvalidateCache("api/products")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _productService.DeleteAsync(id);
        if (!success)
            return NotFound(new { Message = "Product not found." });

        return NoContent();
    }
}