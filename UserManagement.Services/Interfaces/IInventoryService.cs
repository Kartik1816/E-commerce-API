using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Services.Interfaces;

public interface IInventoryService
{
    public Task<IActionResult> GetInventoryDetailsAsync(int userId,string timeFilter,string fromDate, string toDate);
}
