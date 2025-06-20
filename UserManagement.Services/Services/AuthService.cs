using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenRepository _tokenRepository;
    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, ITokenRepository tokenRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenRepository = tokenRepository;
    }

    public User GetvalidUser(AuthViewModel authViewModel)
    {
        return _userRepository.GetvalidUser(authViewModel);
    }

    public bool IsUserPresent(string email)
    {
        return _userRepository.IsUserPresent(email);
    }

    public Role GetRoleById(int roleId)
    {
        return _roleRepository.GetRoleById(roleId);
    }

    public bool SaveToken(Refreshtoken token)
    {
        return _tokenRepository.SaveToken(token);
    }

    public Refreshtoken GetRefreshtoken(string token)
    {
        return _tokenRepository.GetRefreshtoken(token);
    }

    public async Task<IActionResult> RegisterUserAsync(RegistrationViewModel registrationViewModel)
    {
        return await _userRepository.RegisterUserAsync(registrationViewModel);
    }
    
}
