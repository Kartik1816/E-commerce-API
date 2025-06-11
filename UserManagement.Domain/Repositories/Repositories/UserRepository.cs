using Microsoft.AspNetCore.Mvc;
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
            return _userDbContext.Users.Any(u => u.Email == email);
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching user" + e);
        }
    }
    public async Task<IActionResult> RegisterUserAsync(RegistrationViewModel registrationViewModel)
    {
        try
        {
            if (registrationViewModel == null)
            {
                return new BadRequestObjectResult(new { success = false, message = "Invalid registration data" });
            }
            if (_userDbContext.Users.Any(u => u.Email == registrationViewModel.Email))
            {
                return new BadRequestObjectResult(new { success = false, message = "User already exists" });
            }
            User user = new User
            {
                FirstName = registrationViewModel.FirstName,
                LastName = registrationViewModel.LastName,
                Email = registrationViewModel.Email,
                Password = registrationViewModel.Password,
                RoleId = registrationViewModel.RoleId,
                CreatedAt = DateTime.Now,
                CreatedBy = 1,
                Phone = registrationViewModel.PhoneNumber,
                ImageUrl = registrationViewModel.ImageUrl,
                Address = registrationViewModel.Address
            };
            _userDbContext.Users.Add(user);
            await _userDbContext.SaveChangesAsync();
            if (user.Id <= 0)
            {
                return new BadRequestObjectResult(new { success = false, message = "User registration failed" });
            }
            return new OkObjectResult(new { success = true, message = "User registered successfully" });
        }
        catch (Exception)
        {
            throw new Exception("An Exception occured while registering user");
        }
    }
}
