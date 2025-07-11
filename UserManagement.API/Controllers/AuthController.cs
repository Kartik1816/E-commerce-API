using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Models;
using UserManagement.Domain.utils;
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
    private readonly ResponseHandler _responseHandler;
    private readonly IValidationService _validationService;
    public AuthController(IAuthService authService, IGenerateJwt generateJwt, ResponseHandler responseHandler, IValidationService validationService)
    {
        _authService = authService;
        _generateJwt = generateJwt;
        _responseHandler = responseHandler;
        _validationService = validationService;
    }

    //Login API
    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthViewModel authViewModel)
    {
        List<ValidationError> errors = _validationService.ValidateAuthModel(authViewModel);
        if (errors.Any())
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid,CustomErrorMessage.InvalidAuthModel, errors));
        }
        
        bool isUserPresent = _authService.IsUserPresent(authViewModel.Email);
        if (!isUserPresent)
        {
            return NotFound(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotRegistered, CustomErrorMessage.UserNotRegistered,errors));
        }
        User user = _authService.GetvalidUser(authViewModel);
        if (user != null)
        {
            int roleId = user.RoleId;
            Role role = _authService.GetRoleById(roleId);
            string token = _generateJwt.GenerateJwtToken(user, role.Name);
            string refreshToken = _generateJwt.GenerateRefreshToken();
            HttpContext.Response.Cookies.Append(
                "token",
                token,
                new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                });
            HttpContext.Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });
            Refreshtoken refreshTokenEntity = new Refreshtoken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpireTime = DateTime.Now.AddMinutes(30)
            };
            bool isTokenSaved = _authService.SaveToken(refreshTokenEntity);
            
            return Ok(_responseHandler.Success(CustomErrorMessage.LoginSuccess, new { token = token, refreshToken = refreshToken }));
        }
        else
        {
            return Ok(new ResponseModel
            {   
                IsSuccess = false,
                StatusCode = 401,
                Message = CustomErrorMessage.IncorrectCredentials,
                ErrorCode = CustomErrorCode.IncorrectCredentials
            });
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

        refreshtoken.ExpireTime = DateTime.Now.AddMinutes(30);

        bool isTokenUpdated = _authService.SaveToken(refreshtoken);

        HttpContext.Response.Cookies.Append(
                "token", 
                newJwtToken,
                new CookieOptions
                {
                    Path="/",
                    SameSite = SameSiteMode.Lax, 
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                });
                HttpContext.Response.Cookies.Append(
                "refreshToken", 
                newRefreshToken,
                new CookieOptions
                {
                    Path="/",
                    SameSite = SameSiteMode.Lax, 
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                });

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
    public async Task<IActionResult> Registration([FromForm] RegistrationViewModel registrationViewModel)
    {
        List<ValidationError> errors = _validationService.ValidateRegistrationModel(registrationViewModel);
        if (errors.Any())
        {
            return BadRequest(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidRegistrationModel, errors));
        }
        if (registrationViewModel.Image != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(registrationViewModel.Image.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile-images", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                registrationViewModel.Image.CopyTo(fileStream);
            }

            registrationViewModel.ImageUrl = fileName;
        }
        return await _authService.RegisterUserAsync(registrationViewModel);
    }
    
}
