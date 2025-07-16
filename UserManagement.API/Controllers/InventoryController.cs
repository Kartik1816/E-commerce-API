using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("get-inventory-details/{userId}")]
    public async Task<IActionResult> GetInventoryDetails(int userId, string? timeFilter = null, string? fromDate = null, string? toDate = null)
    {
         return await _inventoryService.GetInventoryDetailsAsync(userId,timeFilter,fromDate, toDate);
    }
}
