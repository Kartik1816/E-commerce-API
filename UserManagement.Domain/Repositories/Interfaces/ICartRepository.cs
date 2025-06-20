using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface ICartRepository
{
    public Task<IActionResult> AddProductToCart(CartModel cartModel);

    public Task<IActionResult> RemoveProductFromCart(CartModel cartModel);

    public Task<IActionResult> GetCartProducts(int userId);

    public Task<IActionResult> IncreaseQuantity(CartModel cartModel);

    public Task<IActionResult> DecreaseQuantity(CartModel cartModel);
}
