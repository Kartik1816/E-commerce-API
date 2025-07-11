using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface ICLAService
{
    public Task<List<CategoryViewModel>> GetAllCategoriesAsync();

    public Task<PaginatedResponse<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int userId, PaginationRequestModel paginationRequest);

    public Task<IActionResult> SaveProduct(ProductViewModel productViewModel);

    public Task<IActionResult> GetProductDetails(int productId);

    public Task<IActionResult> DeleteProduct(int productId);

    public Task<IActionResult> GetProductDetailsWithWishListDetails(int productId, int userId);

    public Task<IActionResult> SubscribeUser(string email);

    public Task<IActionResult> GetMinMaxDiscount();

    public Task<SubscribedUsersModel> GetAllSubScribedUsers();

    public Task<IActionResult> GetOfferedProducts();

    public Task<List<ProductViewModel>> GetTopFiveOfferedProducts();
    
    public Task<List<CategoryViewModel>> GetReleasedCategoriesAsync();
}
