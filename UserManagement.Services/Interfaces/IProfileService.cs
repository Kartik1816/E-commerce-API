using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IProfileService
{
    public Task<RegistrationViewModel> GetUserProfileAsync(int userId);

    public Task<IActionResult> SaveProfileAsync(EditProfileViewModel editProfileViewModel);
    
}
