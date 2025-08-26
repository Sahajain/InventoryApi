using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetProductsAsync(ProductQueryParameters parameters);
    Task<ProductResponseDto?> GetProductByIdAsync(int id);
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
    Task<List<ProductResponseDto>> GetLowStockProductsAsync();
}