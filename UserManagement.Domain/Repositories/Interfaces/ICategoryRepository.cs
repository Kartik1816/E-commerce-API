using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface ICategoryRepository
{
    public Task<List<CategoryViewModel>> GetAllCategoriesAsync();
}
