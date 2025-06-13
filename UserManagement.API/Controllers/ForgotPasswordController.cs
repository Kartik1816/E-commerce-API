using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Helper;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForgotPasswordController : ControllerBase
{
    private readonly IForgotPasswordService _forgotPasswordService;
    public ForgotPasswordController(IForgotPasswordService forgotPasswordService)
    {
        _forgotPasswordService = forgotPasswordService;
    }
    [HttpPost("forgotpassword")]
    public IActionResult ForgotPassword([FromBody] string Email)
    {
        bool IsUserWithEmailPresent = _forgotPasswordService.IsUserWithEmailPresent(Email);
        if (!IsUserWithEmailPresent)
        {
            return new JsonResult(new { success = false, message = "Email is not registered" });
        }
        int OTP = HelperService.GenerateSixDigitOTP();
        return _forgotPasswordService.SaveOTP(OTP, Email);
    }
    [HttpPost("verify")]
    public IActionResult VerifyOTP([FromBody] OtpViewModel otpViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter correct Data" });
        }
        return _forgotPasswordService.VerifyOTP(otpViewModel);
    }
    [HttpPost("resetpassword")]
    public IActionResult ResetPassword([FromBody] ResetPasswordViewModel resetPasswordViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter correct Data" });
        }
        return _forgotPasswordService.ResetPassword(resetPasswordViewModel);
    }
}
