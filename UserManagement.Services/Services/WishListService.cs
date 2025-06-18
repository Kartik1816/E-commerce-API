using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class WishListService : IWishListService
{
    private readonly IWishListRepository _wishListRepository;

    public WishListService(IWishListRepository wishListService)
    {
        _wishListRepository = wishListService;
    }

    public async Task<IActionResult> AddRemoveProductToFromWishList(WishListModel wishListModel)
    {
        return await _wishListRepository.AddRemoveProductToFromWishList(wishListModel);
    }

    public async Task<IActionResult> GetUserWishListProducts(int userId)
    {
        return await _wishListRepository.GetUserWishListProducts(userId);
    }
}
