using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    private readonly ResponseHandler _responseHandler;
    public CategoryController(ICategoryService categoryService, ResponseHandler responseHandler)
    {
        _responseHandler = responseHandler;
        _categoryService = categoryService;
    }

    [HttpPost("save-category")]
    public async Task<IActionResult> SaveCategory([FromBody] CategoryViewModel categoryViewModel)
    {
        if (categoryViewModel == null)
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryModel, new List<ValidationError>()));
        }
        return await _categoryService.SaveCategoryAsync(categoryViewModel);
    }

    [HttpPost("delete-category/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (id <= 0)
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryId, new List<ValidationError>()));
        }
        return await _categoryService.DeleteCategoryAsync(id);
    }
}
