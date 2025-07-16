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
            return new OkObjectResult(_responseHandler.Success(isNewProduct ? CustomErrorMessage.ProductAddSuccess : CustomErrorMessage.ProductUpdateSuccess, productOfferModel));

        }
        catch (Exception ex)
        {
            throw new Exception(CustomErrorMessage.ProductSaveError + ex);
        }
    }

    public async Task<PaginatedResponse<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int userId, PaginationRequestModel paginationRequestModel)
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

    public async Task<IActionResult> GetInventoryDetailsAsync(int userId, string timeFilter,string fromDate,string toDate)
    {
        try
        {
            var inventoryViewModel = new InventoryViewModel();

            // 1. Get all products for the seller
            List<Product> sellerProducts = await _userDbContext.Products
                .Where(p => p.CreatedBy == userId)
                .ToListAsync();

            if (!sellerProducts.Any())
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.ProductNotFound, CustomErrorMessage.NoProductsFound, null));
            }

            // Prepare query for sold products
            var orderQuery = _userDbContext.OrderProducts
                .Where(oi => oi.Product.CreatedBy == userId && oi.Order.Status == "Paid")
                .Include(oi => oi.Order);

            // 2. Loop through each product and calculate sold, revenue, last sold
            foreach (Product product in sellerProducts)
            {
                List<OrderProduct> soldOrderItems = await _userDbContext.OrderProducts
                    .Where(oi => oi.ProductId == product.Id && oi.Order.Status == "Paid")
                    .Include(oi => oi.Order)
                    .ToListAsync();

                int soldQuantity = soldOrderItems.Sum(oi => oi.Quantity) ?? 0;
                decimal revenue = soldOrderItems.Sum(oi => oi.Price * oi.Quantity) ?? 0;
                DateTime lastSoldDate = soldOrderItems.OrderByDescending(oi => oi.Order.CreatedAt)
                                                .Select(oi => (DateTime?)oi.Order.CreatedAt)
                                                .FirstOrDefault() ?? DateTime.MinValue;

                inventoryViewModel.InventoryItems.Add(new InventoryItemViewModel
                {
                    ProductName = product.Name,
                    TotalStock = product.Quantity ?? 0,
                    SoldQuantity = soldQuantity,
                    Revenue = revenue,
                    LastSoldDate = lastSoldDate
                });

                inventoryViewModel.TotalStock += product.Quantity ?? 0;
            }

            // 3. Calculate remaining stock initially (total stock - total sold)
            inventoryViewModel.TotalRemainingStock = inventoryViewModel.TotalStock - inventoryViewModel.TotalSoldQuantity;

            // 4. Filter + generate chart
            var today = DateTime.Now.Date;
            DateTime startDate;
            var revenueMap = new Dictionary<string, decimal>();
            var chartLabels = new List<string>();

            if (timeFilter == "last7days")
            {
                startDate = today.AddDays(-6);

                var daysOfWeek = Enumerable.Range(0, 7)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                chartLabels = daysOfWeek.Select(d => d.ToString("ddd")).ToList();

                var weeklySales = await orderQuery
                    .Where(oi => oi.Order.CreatedAt.Date >= startDate && oi.Order.CreatedAt.Date <= today)
                    .GroupBy(oi => oi.Order.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(x => x.Price * x.Quantity)
                    })
                    .ToListAsync();

                foreach (var item in daysOfWeek)
                {
                    string label = item.ToString("ddd");
                    var sale = weeklySales.FirstOrDefault(x => x.Date == item);
                    revenueMap[label] = sale?.Revenue ?? 0;
                }

                inventoryViewModel.InventoryItems = inventoryViewModel.InventoryItems
                    .Where(item => item.LastSoldDate.Date >= startDate)
                    .ToList();
            }
            else if (timeFilter == "last30days")
            {
                startDate = today.AddDays(-29);

                var dateList = Enumerable.Range(0, 30)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                chartLabels = dateList.Select(d => d.ToString("dd MMM")).ToList();

                var dailySales = await orderQuery
                    .Where(oi => oi.Order.CreatedAt.Date >= startDate && oi.Order.CreatedAt.Date <= today)
                    .GroupBy(oi => oi.Order.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(x => x.Price * x.Quantity)
                    })
                    .ToListAsync();

                foreach (var date in dateList)
                {
                    string label = date.ToString("dd MMM");
                    var sale = dailySales.FirstOrDefault(x => x.Date == date);
                    revenueMap[label] = sale?.Revenue ?? 0;
                }

                inventoryViewModel.InventoryItems = inventoryViewModel.InventoryItems
                    .Where(item => item.LastSoldDate.Date >= startDate)
                    .ToList();
            }
            else if (timeFilter == "custom")
            {
                if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate) ||
                    !DateTime.TryParse(fromDate, out DateTime from) || !DateTime.TryParse(toDate, out DateTime to))
                {
                    return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidDateRange, null));
                }

                startDate = from.Date;
                var endDate = to.Date;

                if (startDate > endDate)
                {
                    return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.StartDateAfterEndDate, null));
                }

                var customDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                chartLabels = customDates.Select(d => d.ToString("dd MMM")).ToList();

                var customSales = await orderQuery
                    .Where(oi => oi.Order.CreatedAt.Date >= startDate && oi.Order.CreatedAt.Date <= endDate)
                    .GroupBy(oi => oi.Order.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(x => x.Price * x.Quantity)
                    })
                    .ToListAsync();

                foreach (var date in customDates)
                {
                    string label = date.ToString("dd MMM");
                    var sale = customSales.FirstOrDefault(x => x.Date == date);
                    revenueMap[label] = sale?.Revenue ?? 0;
                }

                inventoryViewModel.InventoryItems = inventoryViewModel.InventoryItems
                    .Where(item => item.LastSoldDate.Date >= startDate && item.LastSoldDate.Date <= endDate)
                    .ToList();
            }
            else // last12months or all time
            {
                startDate = today.AddMonths(-11);

                var monthLabels = Enumerable.Range(0, 12)
                    .Select(i => today.AddMonths(-11 + i))
                    .ToList();

                chartLabels = monthLabels.Select(m => m.ToString("MMM")).ToList();

                var monthlySales = await orderQuery
                    .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt <= today)
                    .GroupBy(oi => new { oi.Order.CreatedAt.Year, oi.Order.CreatedAt.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(x => x.Price * x.Quantity)
                    })
                    .ToListAsync();

                foreach (var m in monthLabels)
                {
                    string label = m.ToString("MMM");
                    var sale = monthlySales.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                    revenueMap[label] = sale?.Revenue ?? 0;
                }

                inventoryViewModel.InventoryItems = inventoryViewModel.InventoryItems
                    .Where(item => item.LastSoldDate >= startDate)
                    .ToList();
            }

            // 5. Assign final chart data
            inventoryViewModel.ChartLabels = chartLabels;
            inventoryViewModel.ChartData = chartLabels.Select(lbl => revenueMap.ContainsKey(lbl) ? revenueMap[lbl] : 0).ToList();

            // 6. Final summary totals (only filtered data now)
            inventoryViewModel.TotalRevenue = inventoryViewModel.InventoryItems.Sum(i => i.Revenue);
            inventoryViewModel.TotalSoldQuantity = inventoryViewModel.InventoryItems.Sum(i => i.SoldQuantity);
            inventoryViewModel.TotalRemainingStock = inventoryViewModel.InventoryItems.Sum(i => i.TotalStock - i.SoldQuantity);

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.GetInventoryDetailsSuccess, inventoryViewModel));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetInventoryDetailsError + e);
        }
    }
}
