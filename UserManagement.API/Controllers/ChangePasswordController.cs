using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChangePasswordController : ControllerBase
{
    private readonly IChangePasswordService _changePasswordService;
    public ChangePasswordController(IChangePasswordService changePasswordService)
    {
        _changePasswordService = changePasswordService;
    }

    
    /// <summary>
    /// Chnage Password API with ChangePasswordViewModel
    /// </summary>
    /// <param name="changePasswordViewModel"></param>
    /// <returns></returns>
    [HttpPost("updatepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel changePasswordViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter correct Data" });
        }
        return await _changePasswordService.ChangePasswordAsync(changePasswordViewModel);
    }
}
