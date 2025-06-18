using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Models;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IAuthService
{
    public User GetvalidUser(AuthViewModel authViewModel);

    public bool IsUserPresent(string email);

    public Role GetRoleById(int roleId);

    public bool SaveToken(Refreshtoken token);

    public Refreshtoken GetRefreshtoken(string token);

    public Task<IActionResult> RegisterUserAsync(RegistrationViewModel registrationViewModel);
    
}
