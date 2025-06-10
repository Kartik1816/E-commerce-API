using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
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
}
