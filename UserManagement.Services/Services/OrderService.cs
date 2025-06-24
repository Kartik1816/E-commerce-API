using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<int> CreateNewOrder(int userId, decimal amount)
    {
        return await _orderRepository.CreateNewOrder(userId, amount);
    }

    public IActionResult UpdateOrderStatus(int orderId)
    {
        return _orderRepository.UpdateOrderStatus(orderId);
    }
}
