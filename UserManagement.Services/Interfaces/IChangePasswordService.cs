using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IChangePasswordService
{
    public Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel);
}
