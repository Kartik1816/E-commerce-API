using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class InventoryService : IInventoryService
{
    private readonly IProductRepository _productRepository;
    public InventoryService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<IActionResult> GetInventoryDetailsAsync(int userId,string timeFilter,string fromDate,string toDate)
    {
        return await _productRepository.GetInventoryDetailsAsync(userId,timeFilter, fromDate, toDate);
    }
}
