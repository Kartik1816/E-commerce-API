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
    public async Task<RegistrationViewModel> GetUserProfileAsync(int userId)
    {
        try
        {
            User? user = await _userDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return null!;
            }
            RegistrationViewModel registrationViewModel = new RegistrationViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.Phone,
                Address = user.Address ?? string.Empty,
                ImageUrl = user.ImageUrl ?? string.Empty,
                RoleId = user.RoleId
            };
            return registrationViewModel;
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching user profile" + e);
        }
    }
    public async Task<IActionResult> SaveProfileAsync(EditProfileViewModel editProfileViewModel)
    {
        try
        {
            User? user = await _userDbContext.Users.FindAsync(editProfileViewModel.Id);
            if (user == null)
            {
                return new NotFoundObjectResult(new { success = false, message = "User not found" });
            }
            user.FirstName = editProfileViewModel.FirstName;
            user.LastName = editProfileViewModel.LastName;
            user.Email = editProfileViewModel.Email;
            user.Phone = editProfileViewModel.PhoneNumber;
            user.Address = editProfileViewModel.Address;
            user.UpdatedAt = DateTime.Now;
            user.RoleId = editProfileViewModel.RoleId;
            if (editProfileViewModel.ImageUrl != null)
            {
                user.ImageUrl = editProfileViewModel.ImageUrl;
            }
            _userDbContext.Users.Update(user);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Profile updated successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while saving profile" + e);
        }
    }
    public IActionResult SaveOTP(int otp, string email)
    {
        try
        {
            User? user = _userDbContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { success = false, message = "User not found" });
            }
            user.Otp = otp;
            user.OtpExpireTime = DateTime.Now.AddHours(1);
            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
            return new JsonResult(new { success = true, message = "OTP Sent successfully", otp = otp });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while saving OTP" + e);
        }
    }
    public IActionResult VerifyOTP(OtpViewModel otpViewModel)
    {
        try
        {
            User? user = _userDbContext.Users.FirstOrDefault(u => u.Email == otpViewModel.Email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { success = false, message = "User not found" });
            }
            if (user.Otp != otpViewModel.OTP || user.OtpExpireTime < DateTime.Now)
            {
                return new BadRequestObjectResult(new { success = false, message = "Invalid or expired OTP" });
            }
            user.Otp = null;
            user.OtpExpireTime = null;
            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
            return new JsonResult(new { success = true, message = "OTP verified successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while verifying OTP" + e);
        }
    }
    public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        try
        {
            User? user = _userDbContext.Users.FirstOrDefault(u => u.Email == resetPasswordViewModel.Email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { success = false, message = "User not found" });
            }
            user.Password = resetPasswordViewModel.NewPassword;
            user.Otp = null;
            user.OtpExpireTime = null;
            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
            return new JsonResult(new { success = true, message = "Password reset successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while resetting password" + e);
        }
    }
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel)
    {
        try
        {
            User? user = await _userDbContext.Users.FindAsync(changePasswordViewModel.Id);
            if (user == null)
            {
                return new NotFoundObjectResult(new { success = false, message = "User not found" });
            }
            if (user.Password != changePasswordViewModel.OldPassword)
            {
                return new JsonResult(new { success = false, message = "Old password is incorrect" });
            }
            if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmPassword)
            {
                return new JsonResult(new { success = false, message = "New password and confirm password do not match" });
            }
            user.Password = changePasswordViewModel.NewPassword;
            _userDbContext.Users.Update(user);
            await _userDbContext.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Password changed successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while changing password" + e);
        }
    }
}
