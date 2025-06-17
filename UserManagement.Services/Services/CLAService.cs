using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class CLAService : ICLAService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CLAService(ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllCategoriesAsync();
    }
    public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _productRepository.GetProductsByCategoryAsync(categoryId);
    }
    public async Task<IActionResult> SaveProduct(ProductViewModel productViewModel)
    {
        return await _productRepository.SaveProductAsync(productViewModel);
    }
    public async Task<IActionResult> GetProductDetails(int productId)
    {
        return await _productRepository.GetProductDetails(productId);
    }
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        return await _productRepository.DeleteProduct(productId);
    }
}
