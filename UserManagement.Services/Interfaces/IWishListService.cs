using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IWishListService
{
    public Task<IActionResult> AddRemoveProductToFromWishList(WishListModel wishListModel);

    public Task<IActionResult> GetUserWishListProducts(int userId);
}
