using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface ICategoryRepository
{
    public Task<List<CategoryViewModel>> GetAllCategoriesAsync();
    public Task<IActionResult> SaveCategoryAsync(CategoryViewModel categoryViewModel);
    public Task<IActionResult> DeleteCategoryAsync(int id);
    public Task<List<CategoryViewModel>> GetReleasedCategoriesAsync();
    public Task<CategoryViewModel> GetCategoryByIdAsync(int id);
}
