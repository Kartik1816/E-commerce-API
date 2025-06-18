using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IWishListRepository
{
    public Task<IActionResult> AddRemoveProductToFromWishList(WishListModel wishListModel);

    public Task<IActionResult> GetUserWishListProducts(int userId);
}
