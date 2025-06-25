using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    public EditProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfile(int userId)
    {
        RegistrationViewModel userProfile = await _profileService.GetUserProfileAsync(userId);
        if (userProfile == null)
        {
            return NotFound(new { success = false, message = "User profile not found" });
        }
        return new JsonResult(new { success = true, data = userProfile });
    }
    [HttpPost("saveProfile")]
    public async Task<IActionResult> SaveProfile([FromBody] EditProfileViewModel editProfileViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter correct Data" });
        }
        return await _profileService.SaveProfileAsync(editProfileViewModel);
    }
}
