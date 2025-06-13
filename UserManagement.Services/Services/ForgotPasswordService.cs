using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class ForgotPasswordService : IForgotPasswordService
{
    private readonly IUserRepository _userRepository;
    public ForgotPasswordService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public bool IsUserWithEmailPresent(string email)
    {
        return _userRepository.IsUserPresent(email);
    }
    public IActionResult SaveOTP(int otp, string email)
    {
        return _userRepository.SaveOTP(otp, email);
    }
    public IActionResult VerifyOTP(OtpViewModel otpViewModel)
    {
        return _userRepository.VerifyOTP(otpViewModel);
    }
    public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        return _userRepository.ResetPassword(resetPasswordViewModel);
    }
}
