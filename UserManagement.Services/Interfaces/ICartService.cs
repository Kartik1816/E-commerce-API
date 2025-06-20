using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface ICartService
{
    public Task<IActionResult> AddProductToCart(CartModel cartModel);

    public Task<IActionResult> RemoveProductFromCart(CartModel cartModel);

    public Task<IActionResult> GetCartProducts(int userId);

    public Task<IActionResult> IncreaseQuantity(CartModel cartModel);

    public Task<IActionResult> DecreaseQuantity(CartModel cartModel);
}
