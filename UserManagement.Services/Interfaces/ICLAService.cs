using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface ICLAService
{
    public Task<List<CategoryViewModel>> GetAllCategoriesAsync();
    public Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId);
    public Task<IActionResult> SaveProduct(ProductViewModel productViewModel);
}
