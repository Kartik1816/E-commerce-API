using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Hubs;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly UserDbContext _userDbContext;

    private readonly IHubContext<NotificationHub> _hubContext;

    private readonly ResponseHandler _responseHandler;

    private readonly PaginationService _paginationService;

    public ProductRepository(UserDbContext userDbContext, IHubContext<NotificationHub> hubContext, ResponseHandler responseHandler, PaginationService paginationService)
    {
        _userDbContext = userDbContext;
        _hubContext = hubContext;
        _responseHandler = responseHandler;
        _paginationService = paginationService;
    }

    public async Task<IActionResult> SaveProductAsync(ProductViewModel productViewModel)
    {
        if (productViewModel == null)
        {
            return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidProductModel, null));
        }
        try
        {

            IQueryable<Product> duplicateCheckQuery = _userDbContext.Products
                .Where(p => p.Name.ToLower() == productViewModel.Name.ToLower());

            if (productViewModel.Id > 0)
            {
                duplicateCheckQuery = duplicateCheckQuery.Where(p => p.Id != productViewModel.Id);
            }

            if (await duplicateCheckQuery.AnyAsync())
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.ProductNameAlreadyExists, null));
            }

            Product product;
            bool isNewProduct = productViewModel.Id <= 0;

            if (isNewProduct)
            {
                string newProductCode = RandomCodeGenerator.GenerateRandomCode();
                while (await _userDbContext.Products.AnyAsync(p => p.ProductCode == newProductCode))
                {
                    newProductCode = RandomCodeGenerator.GenerateRandomCode();
                }

                product = new Product
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = productViewModel.UserId,
                    ProductCode = newProductCode
                };
                _userDbContext.Products.Add(product);
            }
            else
            {
                product = await _userDbContext.Products.FirstOrDefaultAsync(p => p.Id == productViewModel.Id);
                if (product == null)
                {
                    return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.IsValid, CustomErrorMessage.ProductNotFound, null));
                }

                product.UpdatedAt = DateTime.Now;
                product.UpdatedBy = productViewModel.UserId;
            }


            product.Name = productViewModel.Name;
            product.Rate = productViewModel.Price;
            product.Description = productViewModel.Description;
            product.CategoryId = productViewModel.CategoryId;
            product.Discount = productViewModel.Discount;
            product.Quantity = productViewModel.StockQuantity;

            await _userDbContext.SaveChangesAsync();

            if (productViewModel.ImageUrls != null && productViewModel.ImageUrls.Count > 0)
            {
                product.ImageUrl = productViewModel.ImageUrls.FirstOrDefault();
                foreach (string imageUrl in productViewModel.ImageUrls)
                {
                    ProductImage productImage = new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ProductId = product.Id
                    };
                    _userDbContext.ProductImages.Add(productImage);
                }
            }

            await _userDbContext.SaveChangesAsync();

            productViewModel.Id = product.Id;
            productViewModel.ImageUrl = _userDbContext.ProductImages.FirstOrDefault(pi => pi.ProductId == product.Id)?.ImageUrl;

            if (product.Discount > 0)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveDiscountNotification", new
                {
                    ProductName = product.Name,
                    Discount = product.Discount
                });
            }
            ProductOfferModel productOfferModel = new()
            {
                ProductViewModel = product.Discount > 0 ? productViewModel : null,
                IsOffer = product.Discount > 0
            };
            return new OkObjectResult(_responseHandler.Success(isNewProduct ? CustomErrorMessage.ProductAddSuccess : CustomErrorMessage.ProductUpdateSuccess,productOfferModel));
           
        }
        catch (Exception ex)
        {
            throw new Exception(CustomErrorMessage.ProductSaveError + ex);
        }
    }

    public async Task<PaginatedResponse<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int userId,PaginationRequestModel paginationRequestModel)
    {
        try
        {
            int userRole = _userDbContext.Users.FirstOrDefault(u => u.Id == userId)?.RoleId ?? 0;

            IQueryable<ProductViewModel> query = _userDbContext.Products
                .Where(p => p.CategoryId == categoryId && p.IsDeleted == false)
                .Where(p => userRole == 1 ? p.CreatedBy == userId : true)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Price = p.Rate,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    UserId = p.CreatedBy,
                    Discount = p.Discount ?? 0,
                    StockQuantity = p.Quantity ?? 0,
                })
                .OrderBy(p => p.Id);
            return await _paginationService.GetPaginatedDataAsync(query, paginationRequestModel.PageNumber, paginationRequestModel.PageSize);
        }   
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.FetchProductListSuccess + e.Message);
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
                    UserId = p.CreatedBy,
                    StockQuantity = p.Quantity ?? 0,
                    UniqueCode = p.ProductCode,
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.ProductNotFound, CustomErrorMessage.ProductNotFound, null));
            }

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.ProductDetailsSuccess, product));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.ProductDetailsError + e.Message);
        }
    }

    public async Task<IActionResult> DeleteProduct(int productId)
    {
        try
        {
            Product? product = await _userDbContext.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.ProductNotFound, CustomErrorMessage.ProductNotFound, null));
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
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.ProductDeleteSuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.ProductDeleteError + e);
        }
    }
    public async Task<IActionResult> GetProductDetailsWithWishListDetails(int productId, int userId)
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
                    ImageUrls = _userDbContext.ProductImages
                        .Where(pi => pi.ProductId == p.Id)
                        .Select(pi => pi.ImageUrl)
                        .ToList(),
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
                    StockQuantity = p.Quantity ?? 0,
                    UniqueCode = p.ProductCode,
                    CommentModels = _userDbContext.Reviews
                        .Where(r => r.ProductId == productId)
                        .Select(r => new CommentModel
                        {
                            Comment = r.Comments ?? string.Empty,
                            CreatedAt = r.CreatedAt,
                            UserName = r.User != null ? r.User.FirstName : "Unknown",
                            Rating = r.Rating
                        }).OrderByDescending(r => r.CreatedAt).ToList() ?? new List<CommentModel>()
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.ProductNotFound, CustomErrorMessage.ProductNotFound, null));
            }

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.ProductDetailsWithWishListSuccess, product));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.ProductDetailsWithWishListError + e.Message);
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
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.GetMinMaxDiscountSuccess, newSubscriberModel));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetMinMaxDiscountError + e);
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
                DiscountAmount = Math.Round((decimal)((p.Rate * p.Discount / 100) ?? 0), 2),
                StockQuantity = p.Quantity ?? 0,
                UniqueCode = p.ProductCode,
            }).ToListAsync();

            return new JsonResult(new { data = products });

        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetOfferedProductsError + e);
        }
    }

    public async Task<List<ProductViewModel>> GetTopFiveOfferedProducts()
    {
        try
        {
            return await _userDbContext.Products
                .Where(p => p.IsDeleted == false && p.Discount > 0)
                .OrderByDescending(p => p.Discount)
                .Take(5)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Price = p.Rate,
                    CategoryId = p.CategoryId,
                    ImageUrls = _userDbContext.ProductImages
                        .Where(pi => pi.ProductId == p.Id)
                        .Select(pi => pi.ImageUrl)
                        .ToList(),
                    ImageUrl = p.ImageUrl,
                    Discount = (decimal)(p.Discount ?? 0),
                    DiscountAmount = Math.Round((decimal)((p.Rate * p.Discount / 100) ?? 0), 2),
                    StockQuantity = p.Quantity ?? 0,
                    UniqueCode = p.ProductCode,
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetOfferedProductsError + e);
        }
    }
}
