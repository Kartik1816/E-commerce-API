using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CLAController : ControllerBase
{
    private readonly ICLAService _claService;
    public CLAController(ICLAService claService)
    {
        _claService = claService;
    }
    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories()
    {
        List<CategoryViewModel> categories = await _claService.GetAllCategoriesAsync();
        if (categories == null || !categories.Any())
        {
            return NotFound(new { success = false, message = "No categories found" });
        }
        return Ok(new { success = true, data = categories });
    }
    [HttpGet("products/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        List<ProductViewModel> products = await _claService.GetProductsByCategoryAsync(categoryId);
        if (products == null)
        {
            return NotFound(new { success = false, message = "No products found for this category" });
        }
        return Ok(new { success = true, data = products });
    }

    [HttpPost("saveProduct")]
    public async Task<IActionResult> SaveProduct([FromBody] ProductViewModel productViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter Valid Data" });
        }
        return await _claService.SaveProduct(productViewModel);
    }
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductDetails(int productId)
    {
        if (productId <= 0)
        {
            return new JsonResult(new { success = true, message = "Product not found" });
        }
        return await _claService.GetProductDetails(productId);
    }
    [HttpGet("GetProductDetails")]
    public async Task<IActionResult> GetProductDetailsWithWishList([FromQuery] int productId, [FromQuery] int userId)
    {
        if (productId <= 0 || userId <= 0)
        {
            return new JsonResult(new { success = false, message = "Invalid product or user ID" });
        }
        return await _claService.GetProducGetProductDetailsWithWishListtDetails(productId, userId);
    }
    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        if (productId <= 0)
        {
            return new JsonResult(new { success = true, message = "Product not found" });
        }
        return await _claService.DeleteProduct(productId);
    }
}
