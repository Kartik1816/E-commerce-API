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
        if (products == null || !products.Any())
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
}
