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

    [HttpDelete("delete-category/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (id <= 0)
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryId, new List<ValidationError>()));
        }
        return await _categoryService.DeleteCategoryAsync(id);
    }

    [HttpGet("get-category-by-id/{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryId, new List<ValidationError>()));
        }
        CategoryViewModel? category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.CategoryNotFound, CustomErrorMessage.CategoryNotFound, null));
        }
        return Ok(_responseHandler.Success(CustomErrorMessage.GetCategoryByIdSuccess, category));
    }

    [HttpGet("filter/{ids}")]
    public async Task<IActionResult> GetCategoriesByIds(string ids)
    {
        if (string.IsNullOrEmpty(ids))
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryIds, new List<ValidationError>()));
        }

        List<int> categoryIds = ids.Split(',').Select(id => int.Parse(id)).ToList();
        if (categoryIds.Count == 0)
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryIds, new List<ValidationError>()));
        }
        return await _categoryService.GetCategoriesByIdsAsync(categoryIds);
    }

    [HttpPost("release-category")]
    public async Task<IActionResult> ReleaseCategory([FromBody] int id)
    {
        if (id <= 0)
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryId, new List<ValidationError>()));
        }
        return await _categoryService.ReleaseCategoryAsync(id);
    }
}
