using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.Data;
using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Services;

public class ProductService : IProductService
{
    private readonly InventoryDbContext _context;

    public ProductService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductResponseDto>> GetProductsAsync(ProductQueryParameters parameters)
    {
        var query = _context.Products.Where(p => p.IsActive);

        // Apply category filter
        if (!string.IsNullOrEmpty(parameters.Category))
        {
            query = query.Where(p => p.Category.ToLower() == parameters.Category.ToLower());
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(parameters.Search))
        {
            query = query.Where(p => p.Name.Contains(parameters.Search) || 
                                    (p.Description != null && p.Description.Contains(parameters.Search)));
        }

        // Apply sorting
        query = parameters.SortBy?.ToLower() switch
        {
            "price" => parameters.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "stock" => parameters.SortDescending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
            "category" => parameters.SortDescending ? query.OrderByDescending(p => p.Category) : query.OrderBy(p => p.Category),
            "created" => parameters.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => parameters.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();

        var products = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category,
                IsActive = p.IsActive,
                IsLowStock = p.IsLowStock,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return new PagedResult<ProductResponseDto>
        {
            Items = products,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category,
                IsActive = p.IsActive,
                IsLowStock = p.IsLowStock,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return product;
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            StockQuantity = createProductDto.StockQuantity,
            Category = createProductDto.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            IsActive = product.IsActive,
            IsLowStock = product.IsLowStock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        
        if (product == null)
            return null;

        if (!string.IsNullOrEmpty(updateProductDto.Name))
            product.Name = updateProductDto.Name;
        
        if (!string.IsNullOrEmpty(updateProductDto.Description))
            product.Description = updateProductDto.Description;
        
        if (updateProductDto.Price.HasValue)
            product.Price = updateProductDto.Price.Value;
        
        if (updateProductDto.StockQuantity.HasValue)
            product.StockQuantity = updateProductDto.StockQuantity.Value;
        
        if (!string.IsNullOrEmpty(updateProductDto.Category))
            product.Category = updateProductDto.Category;

        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            IsActive = product.IsActive,
            IsLowStock = product.IsLowStock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        
        if (product == null)
            return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductResponseDto>> GetLowStockProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive && p.StockQuantity < 5)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category,
                IsActive = p.IsActive,
                IsLowStock = p.IsLowStock,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
    }
}