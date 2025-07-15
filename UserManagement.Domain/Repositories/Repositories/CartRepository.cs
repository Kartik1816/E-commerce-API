using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class CartRepository : ICartRepository
{
    private readonly UserDbContext _userDbContext;

    private readonly ResponseHandler _responseHandler;
    public CartRepository(UserDbContext userDbContext, ResponseHandler responseHandler)
    {
        _userDbContext = userDbContext;
        _responseHandler = responseHandler;
    }
    public async Task<IActionResult> AddProductToCart(CartModel cartModel)
    {
        try
        {
            if (cartModel == null)
            {
                return new  BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.NullModel, null));
            }
            ProductCart? productCart = await _userDbContext.ProductCarts.FirstOrDefaultAsync(pc => pc.UserId == cartModel.UserId && pc.ProductId == cartModel.ProductId);

            if (productCart != null)
            {
                return new OkObjectResult(new ResponseModel
                {
                    IsSuccess = false,
                    Message = CustomErrorMessage.CartProductAlreadyExists,
                    Data = null
                });
            }

            // Create and add new cart item
            _userDbContext.ProductCarts.Add(new ProductCart
            {
                UserId = cartModel.UserId,
                ProductId = cartModel.ProductId,
                Quantity = cartModel.Quantity
            });

            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.AddProductToCartSuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.AddProductToCartError + e);
        }
    }

    public async Task<IActionResult> RemoveProductFromCart(CartModel cartModel)
    {
        try
        {
            ProductCart? productCart = await _userDbContext.ProductCarts.Where(pc => pc.UserId == cartModel.UserId && pc.ProductId == cartModel.ProductId).FirstOrDefaultAsync();
            if (productCart == null)
            {
                return new  BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.ProductIsNotInCart, null));
            }
            _userDbContext.ProductCarts.Remove(productCart);
            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.RemoveProductFromCartSuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.RemoveProductFromCartError + e);
        }
    }

    public async Task<IActionResult> GetCartProducts(int userId)
    {
        try
        {
            List<CartProductViewModel> cartProducts = await _userDbContext.ProductCarts
                .Where(pc => pc.UserId == userId)
                .Select(pc => new CartProductViewModel
                {
                    ProductId = pc.ProductId,
                    Quantity = pc.Quantity,
                    ProductName = _userDbContext.Products.Where(p => p.Id == pc.ProductId).Select(p => p.Name).FirstOrDefault() ?? "N/A",
                    Price = pc.Product.Rate,
                    ImageUrl = pc.Product.ImageUrl ?? string.Empty,
                    Discount = pc.Product.Discount ?? 0
                })
                .ToListAsync();

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.CartProductSuccess, cartProducts));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.CartProductError + e);
        }
    }

    public async Task<List<Product>> GetProductDetailsInCart(int userId)
    {
        try
        {
            List<Product> cartProducts = await _userDbContext.Products
            .Where(p => _userDbContext.ProductCarts.Any(pc => pc.ProductId == p.Id && pc.UserId == userId))
            .Include(p=>p.Category)
            .ToListAsync();

            return cartProducts;
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching products" + e);
        }
    }
    public async Task<IActionResult> IncreaseQuantity(CartModel cartModel)
    {
        try
        {
            ProductCart? productCart = await _userDbContext.ProductCarts.Where(pc => pc.UserId == cartModel.UserId && pc.ProductId == cartModel.ProductId).FirstOrDefaultAsync();
            if (productCart == null)
            {
               return new  BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.ProductIsNotInCart, null));
            }
            int TotalStock = _userDbContext.Products.FirstOrDefault(p => p.Id == cartModel.ProductId)?.Quantity ?? 0;
            int TotalSold = _userDbContext.Products.FirstOrDefault(p => p.Id == cartModel.ProductId)?.SoldQuantity ?? 0;

            int remainingStock = TotalStock - TotalSold;
            if (productCart.Quantity >= remainingStock)
            {
                return new  BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.QuantityCannotBeIncreased, null));
            }    
            productCart.Quantity = productCart.Quantity + 1;
            _userDbContext.ProductCarts.Update(productCart);
            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.IncreaseQuantitySuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.IncreaseQuantityError + e);
        }
    }
    public async Task<IActionResult> DecreaseQuantity(CartModel cartModel)
    {
        try
        {
            ProductCart? productCart = await _userDbContext.ProductCarts.Where(pc => pc.UserId == cartModel.UserId && pc.ProductId == cartModel.ProductId).FirstOrDefaultAsync();
            if (productCart == null)
            {
                return new  BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.ProductIsNotInCart, null));
            }
            if (productCart.Quantity == 1)
            {
                return new  BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.QuantityCannotBeZero, null));
            }
            productCart.Quantity = productCart.Quantity - 1;
            _userDbContext.ProductCarts.Update(productCart);
            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.DecreaseQuantitySuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.DecreaseQuantityError + e);
        }
    }
}
