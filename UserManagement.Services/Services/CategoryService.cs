using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<IActionResult> SaveCategoryAsync(CategoryViewModel categoryViewModel)
    {
        return await _categoryRepository.SaveCategoryAsync(categoryViewModel);
    }
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        return await _categoryRepository.DeleteCategoryAsync(id);
    }
    public async Task<CategoryViewModel> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetCategoryByIdAsync(id);
    }
}
