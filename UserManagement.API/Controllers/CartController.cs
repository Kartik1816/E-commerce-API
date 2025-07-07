using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ResponseHandler _responseHandler;

    public CartController(ICartService cartService, ResponseHandler responseHandler)
    {
        _cartService = cartService;
        _responseHandler = responseHandler;
    }

    /// <summary>
    /// Add Product to cart API
    /// </summary>
    /// <param name="cartModel"></param>
    /// <returns></returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddProductToCart([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
        }
        return await _cartService.AddProductToCart(cartModel);
    }


    /// <summary>
    /// Remove product from cart API
    /// </summary>
    /// <param name="cartModel"></param>
    /// <returns></returns>
    [HttpPost("remove")]
    public async Task<IActionResult> RemoveProductFromCart([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
             return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
        }
        return await _cartService.RemoveProductFromCart(cartModel);
    }


    /// <summary>
    /// Getting product list of cart API
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("cartProducts/{userId}")]
    public async Task<IActionResult> GetCartProducts(int userId)
    {
        if (userId <= 0)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
        }
        return await _cartService.GetCartProducts(userId);
    }


    /// <summary>
    /// Increasing product quantity from cart
    /// </summary>
    /// <param name="cartModel"></param>
    /// <returns></returns>
    [HttpPost("increase-quantity")]
    public async Task<IActionResult> IncreaseQuantity([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
        }
        return await _cartService.IncreaseQuantity(cartModel);

    }


    /// <summary>
    /// Decreasing product quantity from cart
    /// </summary>
    /// <param name="cartModel"></param>
    /// <returns></returns>
    [HttpPost("decrease-quantity")]
    public async Task<IActionResult> DecreaseQuantity([FromBody] CartModel cartModel)
    {
        if (cartModel.ProductId <= 0 || cartModel.UserId <= 0)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
        }
        return await _cartService.DecreaseQuantity(cartModel);

    }
}
