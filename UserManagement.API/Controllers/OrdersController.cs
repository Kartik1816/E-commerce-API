using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    private readonly ResponseHandler _responseHandler;
    public OrdersController(IOrderService orderService, ResponseHandler responseHandler)
    {
        _orderService = orderService;
        _responseHandler = responseHandler;
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
           return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
        }
        return await _orderService.SaveCustomerReview(customerReviewModel);
    }
}
