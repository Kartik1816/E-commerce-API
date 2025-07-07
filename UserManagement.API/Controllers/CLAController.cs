using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
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

    private readonly IValidationService _validationService;
    private readonly ResponseHandler _responseHandler;
    public CLAController(ICLAService claService,IValidationService validationService, ResponseHandler responseHandler)
    {
        _validationService = validationService;
        _responseHandler = responseHandler;
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
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.CategoryNotFound, CustomErrorMessage.CategoryNotFound, null));
        }
        return Ok(_responseHandler.Success(CustomErrorMessage.GetAllCategoriesSuccess, categories));
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
        List<ProductViewModel> products = await _claService.GetProductsByCategoryAsync(categoryId, userId);
        if (products == null)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductNotFound, CustomErrorMessage.ProductNotFound, null));
        }
        return Ok(_responseHandler.Success(CustomErrorMessage.FetchProductListSuccess, products));
    }


    /// <summary>
    /// Upsert of Product using ProductViewModel
    /// </summary>
    /// <param name="productViewModel"></param>
    /// <returns></returns>
    [HttpPost("saveProduct")]
    public async Task<IActionResult> SaveProduct([FromBody] ProductViewModel productViewModel)
    {
        List<ValidationError> errors = _validationService.ValidateProductModel(productViewModel);
        if (errors.Any())
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidProductModel, errors));
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
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductNotFound, CustomErrorMessage.ProductNotFound, null));
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
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductUserNotFound, CustomErrorMessage.ProductUserNotFound, null));
        }
        return await _claService.GetProductDetailsWithWishListDetails(productId, userId);
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
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.ProductNotFound, CustomErrorMessage.ProductNotFound, null));
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
        List<ValidationError> errors = _validationService.ValidateUserEmail(email);
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
        return Ok(_responseHandler.Success(CustomErrorMessage.FetchSubscribedUsersSuccess, subscribedUsersModel));
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

    [HttpGet("GetTopFiveOfferedProducts")]
    public async Task<IActionResult> GetTopFiveOfferedProducts()
    {
        List<ProductViewModel> topFiveOfferedProducts = await _claService.GetTopFiveOfferedProducts();
        if (topFiveOfferedProducts == null || !topFiveOfferedProducts.Any())
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.NoOfferedProductNotFound, CustomErrorMessage.NoOfferedProductNotFound, null));
        }
        return Ok(_responseHandler.Success(CustomErrorMessage.GetOfferedProductsSuccess, topFiveOfferedProducts));
    }
}
