using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishListController : ControllerBase
{
    private readonly IWishListService _wishListService;
    private readonly ResponseHandler _responseHandler;

    public WishListController(IWishListService wishListService, ResponseHandler responseHandler)
    {
        _wishListService = wishListService;
        _responseHandler = responseHandler;
    }

    [HttpPost("addRemoveToFromWishList")]
    public async Task<IActionResult> AddRemoveProductToFromWishList([FromBody] WishListModel wishListModel)
    {
        return await _wishListService.AddRemoveProductToFromWishList(wishListModel);
    }
    [HttpGet("wishListProducts/{userId}")]
    public async Task<IActionResult> GetUserWishListProducts(int userId)
    {
        if (userId <= 0)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
        }
        
        return await _wishListService.GetUserWishListProducts(userId);
    }
}
