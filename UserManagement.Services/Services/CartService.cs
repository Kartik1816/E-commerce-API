using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<IActionResult> AddProductToCart(CartModel cartModel)
    {
        return await _cartRepository.AddProductToCart(cartModel);
    }

    public async Task<IActionResult> RemoveProductFromCart(CartModel cartModel)
    {
        return await _cartRepository.RemoveProductFromCart(cartModel);
    }

    public async Task<IActionResult> GetCartProducts(int userId)
    {
        return await _cartRepository.GetCartProducts(userId);
    }
}
