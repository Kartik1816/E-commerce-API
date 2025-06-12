using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IForgotPasswordService
{
    public bool IsUserWithEmailPresent(string email);
    public IActionResult SaveOTP(int otp, string email);
    public IActionResult VerifyOTP(OtpViewModel otpViewModel);
}
