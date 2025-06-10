using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Models;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;
using UserManagement.Services.JWT;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGenerateJwt _generateJwt;

    public AuthController(IAuthService authService, IGenerateJwt generateJwt)
    {
        _authService = authService;
        _generateJwt = generateJwt;
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthViewModel authViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter correct Data" });
        }
        bool isUserPresent = _authService.IsUserPresent(authViewModel.Email);
        if (!isUserPresent)
        {
            return new JsonResult(new { success = false, message = "User not registered" });
        }
        User user = _authService.GetvalidUser(authViewModel);
        if (user != null)
        {
            int roleId = user.RoleId;
            Role role = _authService.GetRoleById(roleId);
            string token = _generateJwt.GenerateJwtToken(user, role.Name);

            return new JsonResult(new { success = true, message = "Login successful", token = token });
        }
        else
        {
            return new JsonResult(new { success = false, message = "Incorrect Password" });
        }
    }
}
