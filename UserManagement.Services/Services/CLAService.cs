using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class CLAService : ICLAService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISubscribedUserRepository _subscribedUserRepository;

    public CLAService(ICategoryRepository categoryRepository, IProductRepository productRepository, ISubscribedUserRepository subscribedUserRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _subscribedUserRepository = subscribedUserRepository;
    }

    public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllCategoriesAsync();
    }
    public async Task<PaginatedResponse<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int userId,PaginationRequestModel paginationRequestModel)
    {
        return await _productRepository.GetProductsByCategoryAsync(categoryId, userId,paginationRequestModel);
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

    public async Task<IActionResult> GetProductDetailsWithWishListDetails(int productId, int userId)
    {
        return await _productRepository.GetProductDetailsWithWishListDetails(productId, userId);
    }

    public async Task<IActionResult> SubscribeUser(string email)
    {
        return await _subscribedUserRepository.SubscribeUser(email);
    }

    public async Task<IActionResult> GetMinMaxDiscount()
    {
        return await _productRepository.GetMinMaxDiscount();
    }

    public async Task<SubscribedUsersModel> GetAllSubScribedUsers()
    {
        return await _subscribedUserRepository.GetAllSubScribedUsers();
    }

    public async Task<IActionResult> GetOfferedProducts()
    {
        return await _productRepository.GetOfferedProducts();
    }

    public Task<List<ProductViewModel>> GetTopFiveOfferedProducts()
    {
        return _productRepository.GetTopFiveOfferedProducts();
    }
}
