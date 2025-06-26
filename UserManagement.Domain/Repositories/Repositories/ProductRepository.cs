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
                decimal OldDiscount = (decimal)(product.Discount ?? 0);

                product.Name = productViewModel.Name;
                product.Rate = productViewModel.Price;
                product.Description = productViewModel.Description;
                product.CategoryId = productViewModel.CategoryId;
                product.UpdatedBy = productViewModel.UserId;
                product.UpdatedAt = DateTime.Now;
                product.Discount = productViewModel.Discount;
                if (productViewModel.ImageUrl != null)
                {
                    product.ImageUrl = productViewModel.ImageUrl;
                }
                _userDbContext.Products.Update(product);
                await _userDbContext.SaveChangesAsync();
                if (OldDiscount < product.Discount)
                {
                    productViewModel.ImageUrl = product.ImageUrl;
                    return new JsonResult(new { success = true, message = "Product Updated successfully", offer = true, data = productViewModel });
                }
                return new JsonResult(new { success = true, message = "Product Updated successfully", offer = false });
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
                    ImageUrl = productViewModel.ImageUrl,
                    Discount = productViewModel.Discount
                };
                _userDbContext.Products.Add(product);
                await _userDbContext.SaveChangesAsync();
                if (product.Discount > 0)
                {
                    productViewModel.Id = product.Id;
                    return new JsonResult(new { success = true, message = "Product Added successfully", offer = true, data = productViewModel });
                }
                return new JsonResult(new { success = true, message = "Product Added successfully", offer = false });

            }
        }
        catch (Exception ex)
        {
            throw new Exception("An Exception occured while saving the product" + ex);
        }
    }

    public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int userId)
    {
        try
        {
            int userRole = _userDbContext.Users.Where(u => u.Id == userId).Select(u => u.RoleId).FirstOrDefault();
            

            return await _userDbContext.Products.Where(p => p.CategoryId == categoryId && p.IsDeleted == false)
            .Where(p => userRole == 1  ? p.CreatedBy == userId : true)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Rate,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                UserId = p.CreatedBy,
                Discount=p.Discount ?? 0
            }).OrderBy(p => p.Id).ToListAsync();
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
            ProductViewModel? product = await _userDbContext.Products
                .Where(p => p.Id == productId)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Price = p.Rate,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    Discount = (decimal)(p.Discount ?? 0),
                    DiscountAmount = Math.Round((decimal)((p.Rate * p.Discount / 100) ?? 0), 2),
                    UserId = p.CreatedBy
                })
                .FirstOrDefaultAsync();

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

            List<ProductCart> userCartItems = _userDbContext.ProductCarts
                    .Where(pc => pc.ProductId == product.Id)
                    .ToList();

            if (userCartItems.Count > 0)
            {
                foreach (ProductCart productcart in userCartItems)
                {
                    _userDbContext.ProductCarts.Remove(productcart);
                    _userDbContext.SaveChanges();
                }
            }
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
            ProductViewModel? product = await _userDbContext.Products
                .Where(p => p.Id == productId)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Price = p.Rate,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    Discount = (decimal)(p.Discount ?? 0),
                    DiscountAmount = Math.Round((decimal)((p.Rate * p.Discount / 100) ?? 0), 2),
                    IsInWishList = _userDbContext.UserWishlists
                        .Where(uw => uw.UserId == userId && uw.ProductId == productId)
                        .Select(uw => uw.IsFavourite ?? false)
                        .FirstOrDefault(),

                    IsInCart = _userDbContext.ProductCarts
                        .Where(uc => uc.UserId == userId && uc.ProductId == productId)
                        .Any(),

                    Rating = _userDbContext.Reviews
                        .Where(r => r.ProductId == productId)
                        .Average(r => (double?)r.Rating) != null 
                        ? Math.Round(_userDbContext.Reviews
                            .Where(r => r.ProductId == productId)
                            .Average(r => r.Rating), 1)
                        : 0, // Default rating when no reviews exist

                    CommentModels = _userDbContext.Reviews
                        .Where(r => r.ProductId == productId)
                        .Select(r => new CommentModel
                        {
                            Comment = r.Comments ?? string.Empty,
                            CreatedAt = r.CreatedAt,
                            UserName = r.User != null ? r.User.FirstName : "Unknown",
                            Rating = r.Rating
                        }).OrderByDescending(r=>r.CreatedAt).ToList() ?? new List<CommentModel>()
                })
                .FirstOrDefaultAsync();

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

    public async Task<IActionResult> GetMinMaxDiscount()
    {
        try
        {
            decimal minDiscount = (decimal)await _userDbContext.Products
                .Where(p => p.IsDeleted == false)
                .MinAsync(p => p.Discount);

            decimal maxDiscount = (decimal)await _userDbContext.Products
                .Where(p => p.IsDeleted == false)
                .MaxAsync(p => p.Discount);

            NewSubscriberModel newSubscriberModel = new NewSubscriberModel
            {
                MinDiscountPercentage = minDiscount,
                MaxDiscountPercentage = maxDiscount
            };
            return new JsonResult(new { success = true, data = newSubscriberModel });
        }
        catch (Exception e)
        {
            throw new Exception("An exception occured while getting minimun and maximum discount" + e);
        }
    }

    public async Task<IActionResult> GetOfferedProducts()
    {
        try
        {
            List<ProductViewModel> products = await _userDbContext.Products.Where(p => p.IsDeleted == false && p.Discount > 0).Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Rate,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                Discount = (decimal)(p.Discount ?? 0),
                DiscountAmount = Math.Round((decimal)((p.Rate * p.Discount / 100) ?? 0), 2)
            }).ToListAsync();

            return new JsonResult(new { data = products });

        }
        catch (Exception e)
        {
            throw new Exception("An exception occured getting offered products" + e);
        }
    }
}
