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

    //Login API
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
            string refreshToken = _generateJwt.GenerateRefreshToken();
            Refreshtoken refreshTokenEntity = new Refreshtoken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpireTime = DateTime.Now.AddMinutes(10)
            };
            bool isTokenSaved = _authService.SaveToken(refreshTokenEntity);
            return new JsonResult(new { success = true, message = "Login successful", token = token, refreshToken = refreshToken });
        }
        else
        {
            return new JsonResult(new { success = false, message = "Incorrect Password" });
        }
    }

    //Chnaging refresh token API
    [HttpPost("refreshtoken")]
    public IActionResult RefreshToken([FromBody] string refreshToken)
    {
        Refreshtoken refreshtoken = _authService.GetRefreshtoken(refreshToken);

        if (refreshtoken == null || refreshtoken.ExpireTime < DateTime.UtcNow)
        {
            return Unauthorized("Invalid refresh token");
        }

        Role role = _authService.GetRoleById(refreshtoken.User.RoleId);

        string newJwtToken = _generateJwt.GenerateJwtToken(refreshtoken.User, role.Name);

        string newRefreshToken = _generateJwt.GenerateRefreshToken();

        refreshtoken.Token = newRefreshToken;

        refreshtoken.ExpireTime = DateTime.Now.AddMinutes(10);

        bool isTokenUpdated = _authService.SaveToken(refreshtoken);

        if (!isTokenUpdated)
        {
            return StatusCode(500, "Failed to update refresh token");
        }

        return new JsonResult(new
        {
            accessToken = newJwtToken,
            refreshToken = newRefreshToken
        });
    }

    //Registration API
    [HttpPost("register")]
    public async Task<IActionResult> Registration([FromBody] RegistrationViewModel registrationViewModel)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new { success = false, message = "Please Enter correct Data" });
        }
        
        return await _authService.RegisterUserAsync(registrationViewModel);
    }
    
}
