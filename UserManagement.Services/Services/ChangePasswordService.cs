using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class ChangePasswordService : IChangePasswordService
{
    private readonly IUserRepository _userRepository;
    public ChangePasswordService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel)
    {
        return await _userRepository.ChangePasswordAsync(changePasswordViewModel);
    }
}
