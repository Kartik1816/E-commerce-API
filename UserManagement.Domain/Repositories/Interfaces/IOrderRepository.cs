using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IOrderRepository
{
    public Task<int> CreateNewOrder(int userId, decimal amount);

    public IActionResult UpdateOrderStatus(int orderId);
}
