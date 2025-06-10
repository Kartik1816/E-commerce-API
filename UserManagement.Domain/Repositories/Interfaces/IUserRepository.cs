using UserManagement.Domain.Models;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IUserRepository
{
    public User GetvalidUser(AuthViewModel authViewModel);
    public bool IsUserPresent(string email);
}
