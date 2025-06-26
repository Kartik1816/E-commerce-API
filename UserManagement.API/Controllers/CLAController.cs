using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;


/// <summary>
/// Computer laptop and accessories CURD  controller 
/// </summary>
[ApiController]
[Route("api/[controller]")]

public class CLAController : ControllerBase
{
    private readonly ICLAService _claService;

    public CLAController(ICLAService claService)
    {
        _claService = claService;
    }

    /// <summary>
    /// Get All Cateogry list API
    /// </summary>
    /// <returns></returns>

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

    /// <summary>
    /// Get  Product List by CategoryId and userRole
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("products")]
    public async Task<IActionResult> GetProductsByCategory([FromQuery] int categoryId, [FromQuery] int userId)
    {
        List<ProductViewModel> products = await _claService.GetProductsByCategoryAsync(categoryId,userId);
        if (products == null)
        {
            return NotFound(new { success = false, message = "No products found for this category" });
        }
        return Ok(new { success = true, data = products });
    }


    /// <summary>
    /// Upsert of Product using ProductViewModel
    /// </summary>
    /// <param name="productViewModel"></param>
    /// <returns></returns>
    [HttpPost("saveProduct")]
    public async Task<IActionResult> SaveProduct([FromBody] ProductViewModel productViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter Valid Data" });
        }
        return await _claService.SaveProduct(productViewModel);
    }


    /// <summary>
    /// Get Product details by it's Id
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductDetails(int productId)
    {
        if (productId <= 0)
        {
            return new JsonResult(new { success = true, message = "Product not found" });
        }
        return await _claService.GetProductDetails(productId);
    }


    /// <summary>
    /// Get Product details with Is in WishList and Is in Cart Information
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("GetProductDetails")]
    public async Task<IActionResult> GetProductDetailsWithWishList([FromQuery] int productId, [FromQuery] int userId)
    {
        if (productId <= 0 || userId <= 0)
        {
            return new JsonResult(new { success = false, message = "Invalid product or user ID" });
        }
        return await _claService.GetProducGetProductDetailsWithWishListtDetails(productId, userId);
    }


    /// <summary>
    /// Soft Delete the Product
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        if (productId <= 0)
        {
            return new JsonResult(new { success = true, message = "Product not found" });
        }
        return await _claService.DeleteProduct(productId);
    }



    /// <summary>
    /// Subscribe user with email for new offer mail
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("subscribe-user")]
    public async Task<IActionResult> SubscribeNewUser([FromBody] string email)
    {
        MailAddress mailAddress = new System.Net.Mail.MailAddress(email);

        if (mailAddress.Address != email)
        {
            return new JsonResult(new { success = true, message = "Inavalid email format" });
        }
        return await _claService.SubscribeUser(email);
    }



    /// <summary>
    /// Get Minimum and Maximum discount percentage 
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetMinMaxDiscount")]
    public async Task<IActionResult> GetMinMaxDiscount()
    {
        return await _claService.GetMinMaxDiscount();
    }



    /// <summary>
    /// Getting the list of all subscribe user
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAllSubscribedUsers")]
    public async Task<IActionResult> GetAllSubScribedUsers()
    {
        SubscribedUsersModel subscribedUsersModel = await _claService.GetAllSubScribedUsers();
        return new JsonResult(new { data = subscribedUsersModel });
    }


    /// <summary>
    /// Get all Product list with discount
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetOfferedProducts")]
    public async Task<IActionResult> GetOfferedProducts()
    {
        return await _claService.GetOfferedProducts();
    }
}
