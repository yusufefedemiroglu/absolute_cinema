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

    // get all products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    //get by id guid.
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { Message = "Product not found." });

        return Ok(product);
    }

    // getbytitleid
    [HttpGet("by-title/{titleId:int}")]
    public async Task<IActionResult> GetByTitleId(int titleId)
    {
        var products = await _productService.GetByTitleIdAsync(titleId);
        return Ok(products);
    }
    // create with dto
    [HttpPost("{titleId:int}")]
    public async Task<IActionResult> Create(int titleId, [FromBody] ProductCreateDto dto)
    {
        var id = await _productService.CreateAsync(titleId, dto);
        return CreatedAtAction(nameof(GetById), new { id }, dto);
    }

    // update with dto
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
    {
        var updated = await _productService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound(new { Message = "Product not found." });

        return NoContent();
    }

    // delete by id
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { Message = "Product not found." });

        return NoContent();
    }
}