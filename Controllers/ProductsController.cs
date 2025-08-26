using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductInventoryAPI.Models;
using ProductInventoryAPI.Services;

namespace ProductInventoryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductsController(
        IProductService productService, 
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Get all products with optional filtering, searching, sorting, and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponseDto>>> GetProducts([FromQuery] ProductQueryParameters parameters)
    {
        try
        {
            var result = await _productService.GetProductsAsync(parameters);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving products.", error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the product.", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct(CreateProductDto createProductDto)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(createProductDto);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new { 
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }) 
                });
            }

            var product = await _productService.CreateProductAsync(createProductDto);
            
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the product.", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        try
        {
            var validationResult = await _updateValidator.ValidateAsync(updateProductDto);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new { 
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }) 
                });
            }

            var product = await _productService.UpdateProductAsync(id, updateProductDto);
            
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the product.", error = ex.Message });
        }
    }

    /// <summary>
    /// Soft delete a product (mark as inactive)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var success = await _productService.DeleteProductAsync(id);
            
            if (!success)
                return NotFound(new { message = $"Product with ID {id} not found." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the product.", error = ex.Message });
        }
    }

    /// <summary>
    /// Get products with low stock (stock quantity < 5)
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<List<ProductResponseDto>>> GetLowStockProducts()
    {
        try
        {
            var products = await _productService.GetLowStockProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving low stock products.", error = ex.Message });
        }
    }
}