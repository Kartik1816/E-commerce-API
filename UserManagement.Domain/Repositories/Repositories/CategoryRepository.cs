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
            return await _userDbContext.Categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description ?? string.Empty
            }).OrderBy(c=>c.Id).ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occurred while fetching categories: " + e.Message);
        }
    }
}
