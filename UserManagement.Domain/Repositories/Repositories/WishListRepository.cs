using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class WishListRepository : IWishListRepository
{
    private readonly UserDbContext _userDbContext;
    private readonly ResponseHandler _responseHandler;

    public WishListRepository(UserDbContext userDbContext, ResponseHandler responseHandler)
    {
        _userDbContext = userDbContext;
        _responseHandler = responseHandler;
    }

    public async Task<IActionResult> AddRemoveProductToFromWishList(WishListModel wishListModel)
    {
        try
        {
            if (wishListModel.UserId > 0 && wishListModel.ProductId > 0 && _userDbContext.Users.Any(u=>u.Id == wishListModel.UserId) && _userDbContext.Products.Any(p=>p.Id == wishListModel.ProductId))
            { 
                UserWishlist? userWishlist = await _userDbContext.UserWishlists.Where(uw => uw.ProductId == wishListModel.ProductId && uw.UserId == wishListModel.UserId).FirstOrDefaultAsync();

                if (userWishlist == null)
                {
                    UserWishlist newUserWishList = new()
                    {
                        UserId = wishListModel.UserId,
                        ProductId = wishListModel.ProductId,
                        IsFavourite = true
                    };

                    _userDbContext.UserWishlists.Add(newUserWishList);
                    await _userDbContext.SaveChangesAsync();
                    return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.AddProductToWishListSuccess, null));
                }
                else
                {
                    userWishlist.IsFavourite = !userWishlist.IsFavourite;
                    _userDbContext.UserWishlists.Update(userWishlist);
                    await _userDbContext.SaveChangesAsync();

                    if (userWishlist.IsFavourite.HasValue && userWishlist.IsFavourite.Value)
                    {
                        return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.AddProductToWishListSuccess, null));
                    }

                    return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.RemoveProductFromWishListSuccess, null));
                }
            }
            else
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
            }
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.SaveUserWishListError + e);
        }
    }

    public async Task<IActionResult> GetUserWishListProducts(int userId)
    {
        try
        {
            List<ProductViewModel> userWishlistProducts = await _userDbContext.UserWishlists
                .Where(uw => uw.UserId == userId && uw.IsFavourite == true && uw.Product.IsDeleted == false)
                .Select(uw => new ProductViewModel
                {
                    Id = uw.Product.Id,
                    Name = uw.Product.Name,
                    Description = uw.Product.Description ?? string.Empty,
                    Price = uw.Product.Rate,
                    CategoryId = uw.Product.CategoryId,
                    ImageUrl = uw.Product.ImageUrl,
                    Discount = uw.Product.Discount ?? 0
                }).ToListAsync();

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.GetUserWishListProductsSuccess, userWishlistProducts));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetUserWishListProductsError + e);
        }
    }
}
