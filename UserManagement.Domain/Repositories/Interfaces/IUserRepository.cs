using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Models;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IUserRepository
{
    public User GetvalidUser(AuthViewModel authViewModel);
    public bool IsUserPresent(string email);
    public Task<IActionResult> RegisterUserAsync(RegistrationViewModel registrationViewModel);
    public Task<RegistrationViewModel> GetUserProfileAsync(int userId);
    public Task<IActionResult> SaveProfileAsync(EditProfileViewModel editProfileViewModel);
    public IActionResult SaveOTP(int otp, string email);
    public IActionResult VerifyOTP(OtpViewModel otpViewModel);
    public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel);
    public Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel);
}
