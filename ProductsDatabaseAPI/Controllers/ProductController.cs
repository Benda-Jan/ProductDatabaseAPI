using Microsoft.AspNetCore.Mvc;
using ProductDatabaseAPI.Models;
using ProductDatabaseAPI.Dtos;
using ProductDatabaseAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace ProductDatabaseAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly ProductService _productService;

    public ProductController(ILogger<ProductController> logger, ProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet]
    [Route("{Id:int}")]
    public async Task<ActionResult<Product>> Get(int Id)
    {
        Product product;
        try
        {
            product = await _productService.Find(Id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.Log(LogLevel.Warning, ex.Message);
            return NotFound(ex.Message);
        }

        return Ok(product);
    }

    [HttpGet]
    public async Task<ActionResult<Product[]>> GetAll()
    {
        var result = await _productService.FindAll();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(ProductInputDto input)
    {
        var result = await _productService.Create(input);
        return CreatedAtAction(nameof(Get), new { result.Id }, result);
    }

    [HttpPut]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(int id, ProductInputDto input)
    {
        var result = await _productService.Update(id, input);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{Id:int}")]
    public async Task<ActionResult> Delete(int Id)
    {
        await _productService.Delete(Id);
        return Ok();
    }

}

