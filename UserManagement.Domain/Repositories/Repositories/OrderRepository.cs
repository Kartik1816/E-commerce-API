using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly UserDbContext _userDbContext;

    private readonly ICartRepository _cartRepository;

    private readonly ResponseHandler _responseHandler;

    public OrderRepository(UserDbContext userDbContext, ICartRepository cartRepository, ResponseHandler responseHandler)
    {
        _userDbContext = userDbContext;
        _cartRepository = cartRepository;
        _responseHandler = responseHandler;
    }
    public async Task<int> CreateNewOrder(int userId, decimal amount)
    {
        using (IDbContextTransaction transaction = _userDbContext.Database.BeginTransaction())
        {
            try
            {
                if (userId <= 0 || amount <= 0)
                {
                    return -1;
                }
                Order order = new()
                {
                    Status = "Pending",
                    Amount = amount,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };
                await _userDbContext.Orders.AddAsync(order);
                await _userDbContext.SaveChangesAsync();

                List<Product> productsInCart = await _cartRepository.GetProductDetailsInCart(userId);

                foreach (Product pc in productsInCart)
                {
                    OrderProduct orderProduct = new()
                    {
                        ProductId = pc.Id,
                        OrderId = order.Id,
                        Price = pc.Rate,
                        Discount = pc.Discount,
                        CategoryName = pc.Category.Name,
                        ProductName = pc.Name,
                        Quantity = _userDbContext.ProductCarts.Where(pct => pct.ProductId == pc.Id && pct.UserId == userId).Select(pct => pct.Quantity).FirstOrDefault()
                    };

                    await _userDbContext.OrderProducts.AddAsync(orderProduct);
                    await _userDbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return order.Id;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception("An Exception occured while creating order" + e);
            }
        }
    }

    public IActionResult UpdateOrderStatus(int orderId)
    {
        using (IDbContextTransaction transaction = _userDbContext.Database.BeginTransaction())
        {
            try
            {
                Order? order = _userDbContext.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Order not found" });
                }
                order.Status = "Paid";
                _userDbContext.Orders.Update(order);
                _userDbContext.SaveChanges();

                List<ProductCart> userCartItems = _userDbContext.ProductCarts
                    .Where(pc => pc.UserId == order.CreatedBy)
                    .ToList();
                if (userCartItems.Count > 0)
                {
                    foreach (ProductCart product in userCartItems)
                    {
                        _userDbContext.ProductCarts.Remove(product);
                        _userDbContext.SaveChanges();
                    }
                }

                transaction.CommitAsync();
                return new JsonResult(new { success = true, message = "Order Paid successfully" });
            }
            catch (Exception e)
            {
                transaction.RollbackAsync();
                throw new Exception("An Exception occured while updating order" + e);
            }
        }
    }

    public async Task<IActionResult> GetUsersOrder(int userId)
    {
        try
        {
            List<OrderDetailsViewModel>
             orders = await _userDbContext.Orders
                .Where(o => o.CreatedBy == userId)
                .Select(o => new OrderDetailsViewModel
                {
                    OrderId = o.Id,
                    Status = o.Status ?? string.Empty,
                    TotalAmount = o.Amount ?? 0,
                    UserId = o.CreatedBy,
                    CreatedAt = o.CreatedAt,
                    OrderProductViewModels = o.OrderProducts.Select(op => new OrderProductViewModel
                    {
                        ProductId = op.ProductId,
                        ProductName = op.Product.Name,
                        CategoryName = op.Product.Category.Name,
                        Quantity = op.Quantity ?? 0,
                        Price = op.Price ?? 0,
                        Discount = op.Discount ?? 0,
                        ImageUrl = op.Product.ImageUrl ?? string.Empty
                    }).ToList()
                })
                .ToListAsync();

            UserOrders userOrders = new()
            {
                OrderDetailsViewModels = orders
            };
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.GetUserOrdersSuccess, userOrders));
            
        }
        catch (Exception ex)
        {
            throw new Exception(CustomErrorMessage.GetUserOrdersError + ex.Message);
        }
    }

    public async Task<IActionResult> SaveCustomerReview(CustomerReviewModel customerReviewModel)
    {
        try
        {
            if (customerReviewModel.ProductId <= 0 || customerReviewModel.UserId <= 0)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
            }
            Review review = new()
            {
                UserId = customerReviewModel.UserId,
                ProductId = customerReviewModel.ProductId,
                Rating = (short)customerReviewModel.Rating,
                Comments = customerReviewModel.Comment,
                CreatedAt = DateTime.Now
            };

            _userDbContext.Reviews.Add(review);
            await _userDbContext.SaveChangesAsync();

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.SaveCustomerReviewSuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.SaveCustomerReviewError + e);
        }
    }
}
