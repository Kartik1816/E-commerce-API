using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishListController : ControllerBase
{
    private readonly IWishListService _wishListService;

    public WishListController(IWishListService wishListService)
    {
        _wishListService = wishListService;
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
            return new JsonResult(new { success = false, message = "Uset Not Found" });
        }
        
        return await _wishListService.GetUserWishListProducts(userId);
    }
}
