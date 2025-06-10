using UserManagement.Domain.Models;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IAuthService
{
    public User GetvalidUser(AuthViewModel authViewModel);
    public bool IsUserPresent(string email);
    public Role GetRoleById(int roleId);

}
