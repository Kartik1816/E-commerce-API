using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<IActionResult> SaveProductAsync(ProductViewModel productViewModel);

    public Task<PaginatedResponse<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int userId,PaginationRequestModel paginationRequestModel);

    public Task<IActionResult> GetProductDetails(int productId);

    public Task<IActionResult> DeleteProduct(int productId);

    public Task<IActionResult> GetProductDetailsWithWishListDetails(int productId, int userId);

    public Task<IActionResult> GetMinMaxDiscount();

    public Task<IActionResult> GetOfferedProducts();
    
    public Task<List<ProductViewModel>> GetTopFiveOfferedProducts();
}
