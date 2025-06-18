using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly UserDbContext _userDbContext;

    public ProductRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<IActionResult> SaveProductAsync(ProductViewModel productViewModel)
    {
        try
        {
            if (productViewModel == null)
            {
                return new JsonResult(new { success = false, message = "Product Details Required" });
            }
            if (productViewModel.Id > 0)
            {
                if (await _userDbContext.Products.AnyAsync(p => p.Name == productViewModel.Name && p.Id != productViewModel.Id))
                {
                    return new JsonResult(new { success = false, message = "Product with name already exist" });
                }
                Product? product = await _userDbContext.Products.FirstOrDefaultAsync(p => p.Id == productViewModel.Id);
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Product object not found" });
                }
                product.Name = productViewModel.Name;
                product.Rate = productViewModel.Price;
                product.Description = productViewModel.Description;
                product.CategoryId = productViewModel.CategoryId;
                product.UpdatedBy = productViewModel.UserId;
                product.UpdatedAt = DateTime.Now;
                if (productViewModel.ImageUrl != null)
                {
                    product.ImageUrl = productViewModel.ImageUrl;
                }
                _userDbContext.Products.Update(product);
                await _userDbContext.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Product Updated successfully" });
            }
            else
            {
                if (await _userDbContext.Products.AnyAsync(p => p.Name == productViewModel.Name))
                {
                    return new JsonResult(new { success = false, messager = "Product Already exist" });
                }
                Product product = new()
                {
                    Name = productViewModel.Name,
                    Description = productViewModel.Description,
                    CategoryId = productViewModel.CategoryId,
                    Rate = productViewModel.Price,
                    CreatedAt = DateTime.Now,
                    CreatedBy = productViewModel.UserId,
                    ImageUrl = productViewModel.ImageUrl
                };
                _userDbContext.Products.Add(product);
                await _userDbContext.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Product Added successfully" });

            }
        }
        catch (Exception ex)
        {
            throw new Exception("An Exception occured while saving the product" + ex);
        }
    }

    public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            return await _userDbContext.Products.Where(p => p.CategoryId == categoryId && p.IsDeleted == false).Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Rate,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl
            }).ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occurred while fetching products: " + e.Message);
        }
    }

    public async Task<IActionResult> GetProductDetails(int productId)
    {
        try
        {
            ProductViewModel? product = await _userDbContext.Products.Where(p => p.Id == productId).Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Rate,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                Discount = (decimal)(p.Discount ?? 0),

            }).FirstOrDefaultAsync();

            if (product == null)
            {
                return new JsonResult(new { success = false, message = "Product not found" });
            }

            return new JsonResult(new { success = true, data = product });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occurred while fetching product details: " + e.Message);
        }
    }

    public async Task<IActionResult> DeleteProduct(int productId)
    {
        try
        {
            Product? product = await _userDbContext.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null)
            {
                return new JsonResult(new { success = false, message = "Product not found" });
            }

            product.IsDeleted = true;
            _userDbContext.Products.Update(product);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Product deleted successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An exception occured while deleting the product " + e);
        }
    }
    public async Task<IActionResult> GetProducGetProductDetailsWithWishListtDetails(int productId, int userId)
    {
        try
        {
            ProductViewModel? product = await _userDbContext.Products.Where(p => p.Id == productId).Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Rate,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                Discount = (decimal)(p.Discount ?? 0),
                IsInWishList = _userDbContext.UserWishlists.Where(uw => uw.UserId == userId && uw.ProductId == productId).Select(uw => uw.IsFavourite ?? false).FirstOrDefault(),
                IsInCart = _userDbContext.ProductCarts.Where(uc => uc.UserId == userId && uc.ProductId == productId).Any()
            }).FirstOrDefaultAsync();

            if (product == null)
            {
                return new JsonResult(new { success = false, message = "Product not found" });
            }

            return new JsonResult(new { success = true, data = product });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occurred while fetching product details: " + e.Message);
        }
    }
}
