using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    public ProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<RegistrationViewModel> GetUserProfileAsync(int userId)
    {
        return await _userRepository.GetUserProfileAsync(userId);
    }
    public async Task<IActionResult> SaveProfileAsync(EditProfileViewModel editProfileViewModel)
    {
        return await _userRepository.SaveProfileAsync(editProfileViewModel);
    }
}
