using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IOrderRepository
{
    public Task<int> CreateNewOrder(int userId, decimal amount);

    public IActionResult UpdateOrderStatus(int orderId);

    public Task<IActionResult> GetUsersOrder(int userId);

    public Task<IActionResult> SaveCustomerReview(CustomerReviewModel customerReviewModel);
}
