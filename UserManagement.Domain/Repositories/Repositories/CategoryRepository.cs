using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly UserDbContext _userDbContext;
    public CategoryRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
    {
        try
        {
            List<Category> categories = await _userDbContext.Categories.ToListAsync();
            return categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description ?? string.Empty
            }).ToList();
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occurred while fetching categories: " + e.Message);
        }
    }
}
