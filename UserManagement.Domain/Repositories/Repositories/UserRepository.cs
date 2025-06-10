using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _userDbContext;
    public UserRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public User GetvalidUser(AuthViewModel authViewModel)
    {
        try
        {
            User user = _userDbContext.Users.FirstOrDefault(u => u.Email == authViewModel.Email) ?? new User();
            if (user != null && user.Password == authViewModel.Password)
            {
                return user;
            }
            else
            {
                return null!;
            }
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching user" + e);
        }
    }
    public bool IsUserPresent(string email)
    {
        try
        {
            return  _userDbContext.Users.Any(u => u.Email == email);
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching user" + e);
        }
    }
}
