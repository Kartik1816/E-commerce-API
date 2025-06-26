using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class CartRepository : ICartRepository
{
    private readonly UserDbContext _userDbContext;

    public CartRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public async Task<IActionResult> AddProductToCart(CartModel cartModel)
    {
        try
        {
            ProductCart? productCart = await _userDbContext.ProductCarts.Where(pc => pc.UserId == cartModel.UserId && pc.ProductId == cartModel.ProductId).FirstOrDefaultAsync();
            if (productCart != null)
            {
                return new JsonResult(new { success = false, message = "Product alredy exist in cart" });
            }
            ProductCart newProductInCart = new()
            {
                UserId = cartModel.UserId,
                ProductId = cartModel.ProductId,
                Quantity = cartModel.Quantity
            };
            _userDbContext.ProductCarts.Add(newProductInCart);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Product successfully added into the cart" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while adding product to cart" + e);
        }
    }

    public async Task<IActionResult> RemoveProductFromCart(CartModel cartModel)
    {
        try
        {
            ProductCart? productCart = await _userDbContext.ProductCarts.Where(pc => pc.UserId == cartModel.UserId && pc.ProductId == cartModel.ProductId).FirstOrDefaultAsync();
            if (productCart == null)
            {
                return new JsonResult(new { success = false, message = "Product is not in the cart" });
            }
            _userDbContext.ProductCarts.Remove(productCart);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Product removed from cart successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while adding product to cart" + e);
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

            return new JsonResult(new { success = true, data = cartProducts });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching products" + e);
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
                return new JsonResult(new { success = false, message = "Product is not in the cart" });
            }
            productCart.Quantity = productCart.Quantity + 1;
            _userDbContext.ProductCarts.Update(productCart);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Quantity increased successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while increasing product quantity to cart" + e);
        }
    }
    public async Task<IActionResult> DecreaseQuantity(CartModel cartModel)
    {
        try
        {
            ProductCart? productCart = await _userDbContext.ProductCarts.Where(pc => pc.UserId == cartModel.UserId && pc.ProductId == cartModel.ProductId).FirstOrDefaultAsync();
            if (productCart == null)
            {
                return new JsonResult(new { success = false, message = "Product is not in the cart" });
            }
            if (productCart.Quantity == 1)
            {
                return new JsonResult(new { success = false, message = "Quantity can not be 0" });
            }
            productCart.Quantity = productCart.Quantity - 1;
            _userDbContext.ProductCarts.Update(productCart);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Quantity decreased successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while increasing product quantity to cart" + e);
        }
    }
}
