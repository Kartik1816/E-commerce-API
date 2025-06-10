using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<IActionResult> SaveProductAsync(ProductViewModel productViewModel,int userId);
}
