using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface ICLAService
{
    public Task<List<CategoryViewModel>> GetAllCategoriesAsync();

    public Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId);

    public Task<IActionResult> SaveProduct(ProductViewModel productViewModel);

    public Task<IActionResult> GetProductDetails(int productId);

    public Task<IActionResult> DeleteProduct(int productId);

    public Task<IActionResult> GetProducGetProductDetailsWithWishListtDetails(int productId,int userId);
    
}
