using Application.DTOs.Product;
using Application.Services;
using Core.Entities;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("by-title/{titleId:int}")]
    public async Task<IActionResult> GetByTitleId(int titleId)
    {
        var products = await _productService.GetByTitleIdAsync(titleId);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto product)
    {
        ModelState.Remove("Title");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);


        // Map DTO to Product
        var id = await _productService.CreateAsync(new Product
        {
            TitleId = product.TitleId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock
        });
        return CreatedAtAction(nameof(GetById), new { id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
    {
        var updated = await _productService.UpdateAsync(id, product);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}