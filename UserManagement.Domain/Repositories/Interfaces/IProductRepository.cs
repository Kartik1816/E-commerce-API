using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<IActionResult> SaveProductAsync(ProductViewModel productViewModel);

    public Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId);

    public Task<IActionResult> GetProductDetails(int productId);

    public Task<IActionResult> DeleteProduct(int productId);
    
    public Task<IActionResult> GetProducGetProductDetailsWithWishListtDetails(int productId,int userId);
}
