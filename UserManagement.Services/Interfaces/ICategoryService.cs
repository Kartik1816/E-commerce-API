using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface ICategoryService
{
    public Task<IActionResult> SaveCategoryAsync(CategoryViewModel categoryViewModel);
    public Task<IActionResult> DeleteCategoryAsync(int id);
}
