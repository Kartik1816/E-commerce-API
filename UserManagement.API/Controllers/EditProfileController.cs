using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ResponseHandler _responseHandler;
    private readonly IValidationService _validationService;

    public EditProfileController(IProfileService profileService, ResponseHandler responseHandler, IValidationService validationService)
    {
        _profileService = profileService;
        _responseHandler = responseHandler;
        _validationService = validationService;
    }


    /// <summary>
    /// Get User 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfile(int userId)
    {
        RegistrationViewModel userProfile = await _profileService.GetUserProfileAsync(userId);
        if (userProfile == null)
        {
           return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
        }
        return Ok(_responseHandler.Success(CustomErrorMessage.GetUserProfileSuccess, userProfile));
    }
    [HttpPost("saveProfile")]
    public async Task<IActionResult> SaveProfile([FromBody] EditProfileViewModel editProfileViewModel)
    {
        List<ValidationError> errors = _validationService.ValidateEditProfileModel(editProfileViewModel);
        if(errors.Any())
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.UpdateUserProfileError, errors));
        }
        return await _profileService.SaveProfileAsync(editProfileViewModel);
    }
}
