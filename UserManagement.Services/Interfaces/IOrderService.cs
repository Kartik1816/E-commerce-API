using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IOrderService
{
    public Task<int> CreateNewOrder(int userId, decimal amount);

    public IActionResult UpdateOrderStatus(int orderId);

    public Task<IActionResult> GetUsersOrder(int userId);

    public Task<IActionResult> SaveCustomerReview(CustomerReviewModel customerReviewModel);

    public Task<IActionResult> GetOrderDetails(int orderId);
}
