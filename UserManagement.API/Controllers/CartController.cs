using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddProductToCart([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
            return new JsonResult(new { success = false, message = "Product or User not found" });
        }
        return await _cartService.AddProductToCart(cartModel);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveProductFromCart([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
            return new JsonResult(new { success = false, message = "Product or User not found" });
        }
        return await _cartService.RemoveProductFromCart(cartModel);
    }

    [HttpGet("cartProducts/{userId}")]
    public async Task<IActionResult> GetCartProducts(int userId)
    {
        if (userId <= 0)
        {
            return new JsonResult(new { success = false, message = "User Not Found" });
        }
        return await _cartService.GetCartProducts(userId);
    }

    [HttpPost("increase-quantity")]
    public async Task<IActionResult> IncreaseQuantity([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
            return new JsonResult(new { success = false, message = "Product or User not found" });
        }
        return await _cartService.IncreaseQuantity(cartModel);

    }

    [HttpPost("decrease-quantity")]
    public async Task<IActionResult> DecreaseQuantity([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
            return new JsonResult(new { success = false, message = "Product or User not found" });
        }
        return await _cartService.DecreaseQuantity(cartModel);

    }
}
