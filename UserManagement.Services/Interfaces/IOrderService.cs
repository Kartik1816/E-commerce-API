using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Services.Interfaces;

public interface IOrderService
{
    public Task<int> CreateNewOrder(int userId, decimal amount);

    public IActionResult UpdateOrderStatus(int orderId);
}
