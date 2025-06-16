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
                Product product = await _userDbContext.Products.FirstOrDefaultAsync(p => p.Id == productViewModel.Id) ?? new Product();
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
                Product product = new Product
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
            List<Product> products = await _userDbContext.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Rate,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl
            }).ToList();
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occurred while fetching products: " + e.Message);
        }
    }
}
