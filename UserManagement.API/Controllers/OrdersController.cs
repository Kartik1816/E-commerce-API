using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("get-user-orders/{userId}")]
    public async Task<IActionResult> GetUserOrders(int userId)
    {
        return await _orderService.GetUsersOrder(userId);
    }

    [HttpPost("reviews")]
    public async Task<IActionResult> CustomerReview(CustomerReviewModel customerReviewModel)
    {
        if (customerReviewModel.ProductId <= 0 || customerReviewModel.UserId <= 0)
        {
            return new JsonResult(new { success = false, message = "Invalid User Or Product" });
        }
        return await _orderService.SaveCustomerReview(customerReviewModel);
    }
}
