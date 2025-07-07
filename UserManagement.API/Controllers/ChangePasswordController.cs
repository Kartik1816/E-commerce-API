using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChangePasswordController : ControllerBase
{
    private readonly IChangePasswordService _changePasswordService;
    private readonly IValidationService _validationService;
    private readonly ResponseHandler _responseHandler;
    public ChangePasswordController(IChangePasswordService changePasswordService, IValidationService validationService, ResponseHandler responseHandler)
    {
        _changePasswordService = changePasswordService;
        _validationService = validationService;
        _responseHandler = responseHandler;
    }

    
    /// <summary>
    /// Chnage Password API with ChangePasswordViewModel
    /// </summary>
    /// <param name="changePasswordViewModel"></param>
    /// <returns></returns>
    [HttpPost("updatepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel changePasswordViewModel)
    {
        List<ValidationError> errors = _validationService.ValidateChangePasswordModel(changePasswordViewModel);
        if (errors.Any())
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.ChangePasswordError, errors));
        }
        return await _changePasswordService.ChangePasswordAsync(changePasswordViewModel);
    }
}
