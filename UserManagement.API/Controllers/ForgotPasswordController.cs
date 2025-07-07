using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Helper;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForgotPasswordController : ControllerBase
{
    private readonly IForgotPasswordService _forgotPasswordService;
    private readonly IValidationService _validationService;
    private readonly ResponseHandler _responseHandler;
    public ForgotPasswordController(IForgotPasswordService forgotPasswordService,IValidationService validationService, ResponseHandler responseHandler)
    {
        _validationService = validationService;
        _responseHandler = responseHandler;
        _forgotPasswordService = forgotPasswordService;
    }

    [HttpPost("forgotpassword")]
    public IActionResult ForgotPassword([FromBody] string Email)
    {
        bool IsUserWithEmailPresent = _forgotPasswordService.IsUserWithEmailPresent(Email);
        if (!IsUserWithEmailPresent)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotRegistered, CustomErrorMessage.UserNotRegistered, null));
        }
        int OTP = HelperService.GenerateSixDigitOTP();
        return _forgotPasswordService.SaveOTP(OTP, Email);
    }
    [HttpPost("verify")]
    public IActionResult VerifyOTP([FromBody] OtpViewModel otpViewModel)
    {
        List<ValidationError> errors = _validationService.ValidateOtpModel(otpViewModel);
        if (errors.Any())
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidOtpModel, errors));
        }
        return _forgotPasswordService.VerifyOTP(otpViewModel);
    }
    [HttpPost("resetpassword")]
    public IActionResult ResetPassword([FromBody] ResetPasswordViewModel resetPasswordViewModel)
    {
        List<ValidationError> errors = _validationService.ValidateResetPasswordModel(resetPasswordViewModel);
        if (errors.Any())
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidResetPasswordModel, errors));
        }
        return _forgotPasswordService.ResetPassword(resetPasswordViewModel);
    }
}
