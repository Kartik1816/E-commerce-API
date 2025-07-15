using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly UserDbContext _userDbContext;
    private readonly ResponseHandler _responseHandler;
    public CategoryRepository(UserDbContext userDbContext, ResponseHandler responseHandler)
    {
        _userDbContext = userDbContext;
        _responseHandler = responseHandler;
    }

    public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
    {
        try
        {
            return await _userDbContext.Categories.Where(c => c.IsDeleted == false).Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description ?? string.Empty,
                IsReleased = (bool)(c.IsReleased ?? false)
            }).OrderBy(c => c.Id).ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetAllCategoriesError + e.Message);
        }
    }

    public async Task<IActionResult> SaveCategoryAsync(CategoryViewModel categoryViewModel)
    {
        try
        {
            if (categoryViewModel.Id == 0)
            {
                bool categoryExists = await _userDbContext.Categories.AnyAsync(c => c.Name.ToLower() == categoryViewModel.Name.ToLower());
                if (categoryExists)
                {
                    return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.CategoryAlreadyExists, CustomErrorMessage.CategoryAlreadyExists, null));
                }

                Category newCategory = new Category
                {
                    Name = categoryViewModel.Name,
                    Description = categoryViewModel.Description,
                    CreatedBy = 1,
                    ImageUrl = categoryViewModel.ImageUrl,
                };
                await _userDbContext.Categories.AddAsync(newCategory);
            }
            else
            {
                Category? existingCategory = await _userDbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryViewModel.Id);
                if (existingCategory == null)
                {
                    return new NotFoundObjectResult(CustomErrorMessage.CategoryNotFound);
                }

                existingCategory.Name = categoryViewModel.Name;
                existingCategory.Description = categoryViewModel.Description;
                _userDbContext.Categories.Update(existingCategory);
            }
            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.CategorySavedSuccessfully, null));
        }
        catch (Exception)
        {
            throw new Exception(CustomErrorMessage.CategorySaveError);
        }
    }
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        try
        {
            Category? category = await _userDbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.CategoryNotFound, CustomErrorMessage.CategoryNotFound, null));
            }
            //if there are any products associated with this category, we cannot delete it
            bool hasProducts = await _userDbContext.Products.AnyAsync(p => p.CategoryId == id && p.IsDeleted == false);
            if (hasProducts)
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.CategoryHasProducts, CustomErrorMessage.CategoryHasProducts, null));
            }

            category.IsDeleted = true;
            _userDbContext.Categories.Update(category);
            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.CategoryDeletedSuccessfully, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.CategoryDeleteError + e.Message);
        }
    }

    public Task<List<CategoryViewModel>> GetReleasedCategoriesAsync()
    {
        try
        {
            return _userDbContext.Categories
                .Where(c => c.IsReleased == true && c.IsDeleted == false)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description ?? string.Empty,
                    IsReleased = (bool)(c.IsReleased ?? false),
                    ImageUrl = c.ImageUrl
                })
                .OrderBy(c => c.Id)
                .ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetAllCategoriesError + e.Message);
        }
    }

    public async Task<CategoryViewModel> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await _userDbContext.Categories
                .Where(c => c.Id == id && c.IsDeleted == false)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description ?? string.Empty,
                    IsReleased = (bool)(c.IsReleased ?? false),
                    ImageUrl = c.ImageUrl
                })
                .FirstOrDefaultAsync() ?? throw new Exception(CustomErrorMessage.CategoryNotFound);
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetCategoryByIdError + e.Message);
        }
    }

    public async Task<IActionResult> GetCategoriesByIdsAsync(List<int> categoryIds)
    {
        try
        {
            if (categoryIds == null || !categoryIds.Any())
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidCategoryIds, null));
            }

            List<CategoryViewModel> categories = await _userDbContext.Categories
                .Where(c => categoryIds.Contains(c.Id) && c.IsDeleted == false && c.IsReleased == true)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description ?? string.Empty,
                    IsReleased = (bool)(c.IsReleased ?? false),
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();

            if (!categories.Any())
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.CategoryNotFound, CustomErrorMessage.CategoryNotFound, null));
            }

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.GetCategoriesByIdsSuccess, categories));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetCategoriesByIdsError + e.Message);
        }
    }

    public async Task<IActionResult> ReleaseCategoryAsync(int id)
    {
        try
        {
            Category? category = await _userDbContext.Categories.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
            if (category == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.CategoryNotFound, CustomErrorMessage.CategoryNotFound, null));
            }

            category.IsReleased = true;
            _userDbContext.Categories.Update(category);
            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.CategoryReleasedSuccessfully, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.CategoryReleaseError + e.Message);
        }
    }
}
