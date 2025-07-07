using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _userDbContext;
    private readonly ResponseHandler _responseHandler;
    public UserRepository(UserDbContext userDbContext, ResponseHandler responseHandler)
    {
        _userDbContext = userDbContext;
        _responseHandler = responseHandler;
    }

    public User GetvalidUser(AuthViewModel authViewModel)
    {
        try
        {
            User user = _userDbContext.Users.FirstOrDefault(u => u.Email == authViewModel.Email.ToLower()) ?? new User();
            if (user != null && user.Password == authViewModel.Password)
            {
                return user;
            }
            else
            {
                return null!;
            }
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching user" + e);
        }
    }

    public bool IsUserPresent(string email)
    {
        try
        {
            return _userDbContext.Users.Any(u => u.Email == email);
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching user" + e);
        }
    }

    public async Task<IActionResult> RegisterUserAsync(RegistrationViewModel registrationViewModel)
    {
        try
        {
            if (registrationViewModel == null)
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.InvalidRegistrationModel, null));
            }
            if (_userDbContext.Users.Any(u => u.Email == registrationViewModel.Email))
            {
               return new OkObjectResult(new ResponseModel{
                   IsSuccess = false,
                   Message = CustomErrorMessage.EmailAlreadyExists,
                   Data = null,
                   ErrorCode= CustomErrorCode.EmailAlreadyExists,
                   StatusCode = StatusCodes.Status400BadRequest
               });
            }
            User user = new User
            {
                FirstName = registrationViewModel.FirstName,
                LastName = registrationViewModel.LastName,
                Email = registrationViewModel.Email.ToLower(),
                Password = registrationViewModel.Password,
                RoleId = registrationViewModel.RoleId,
                CreatedAt = DateTime.Now,
                CreatedBy = 1,
                Phone = registrationViewModel.PhoneNumber,
                ImageUrl = registrationViewModel.ImageUrl,
                Address = registrationViewModel.Address
            };
            _userDbContext.Users.Add(user);
            await _userDbContext.SaveChangesAsync();
            if (user.Id <= 0)
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.RegistrationError, null));
            }
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.RegistrationSuccess, new { userId = user.Id }));
        }
        catch (Exception)
        {
            throw new Exception(CustomErrorMessage.RegistrationError);
        }
    }

    public async Task<RegistrationViewModel> GetUserProfileAsync(int userId)
    {
        try
        {
            User? user = await _userDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return null!;
            }
            RegistrationViewModel registrationViewModel = new RegistrationViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.Phone,
                Address = user.Address ?? string.Empty,
                ImageUrl = user.ImageUrl ?? string.Empty,
                RoleId = user.RoleId
            };
            return registrationViewModel;
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.GetUserProfileError + e);
        }
    }

    public async Task<IActionResult> SaveProfileAsync(EditProfileViewModel editProfileViewModel)
    {
        try
        {
            User? user = await _userDbContext.Users.FindAsync(editProfileViewModel.Id);
            if (user == null)
            {
               return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
            }
            user.FirstName = editProfileViewModel.FirstName;
            user.LastName = editProfileViewModel.LastName;
            user.Email = editProfileViewModel.Email;
            user.Phone = editProfileViewModel.PhoneNumber;
            user.Address = editProfileViewModel.Address;
            user.UpdatedAt = DateTime.Now;
            if (editProfileViewModel.ImageUrl != null)
            {
                user.ImageUrl = editProfileViewModel.ImageUrl;
            }
            _userDbContext.Users.Update(user);
            await _userDbContext.SaveChangesAsync();
           return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.UpdateUserProfileSuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.UpdateUserProfileError + e);
        }
    }

    public IActionResult SaveOTP(int otp, string email)
    {
        try
        {
            User? user = _userDbContext.Users.FirstOrDefault(u => u.Email == email.ToLower());
            if (user == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
            }
            user.Otp = otp;
            user.OtpExpireTime = DateTime.Now.AddHours(1);
            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.OTPSentSuccess, otp.ToString()));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.OTPSentError + e);
        }
    }

    public IActionResult VerifyOTP(OtpViewModel otpViewModel)
    {
        try
        {
            User? user = _userDbContext.Users.FirstOrDefault(u => u.Email == otpViewModel.Email);
            if (user == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
            }
            if (user.Otp != otpViewModel.OTP || user.OtpExpireTime < DateTime.Now)
            {
                return new BadRequestObjectResult(new ResponseModel
                {
                    IsSuccess = false,
                    Message = CustomErrorMessage.OTPInvalid,
                    Data = null,
                    ErrorCode = CustomErrorCode.OTPInvalid,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            user.Otp = null;
            user.OtpExpireTime = null;
            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.OTPVerifySuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.OTPVerifyError + e);
        }
    }

    public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        try
        {
            User? user = _userDbContext.Users.FirstOrDefault(u => u.Email == resetPasswordViewModel.Email);
            if (user == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
            }
            user.Password = resetPasswordViewModel.NewPassword;
            user.Otp = null;
            user.OtpExpireTime = null;
            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.PasswordResetSuccess, null)); 
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.PasswordResetError + e);
        }
    }

    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel)
    {
        try
        {
            User? user = await _userDbContext.Users.FindAsync(changePasswordViewModel.Id);
            if (user == null)
            {
                return new NotFoundObjectResult(_responseHandler.NotFoundRequest(CustomErrorCode.UserNotFound, CustomErrorMessage.UserNotFound, null));
            }
            if (user.Password != changePasswordViewModel.OldPassword)
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.IncorrectOldPassword, null));
            }
            if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmPassword)
            {
                return new BadRequestObjectResult(_responseHandler.BadRequest(CustomErrorCode.IsValid, CustomErrorMessage.NewAndConfirmPasswordMismatch, null));
            }
            user.Password = changePasswordViewModel.NewPassword;
            _userDbContext.Users.Update(user);
            await _userDbContext.SaveChangesAsync();
            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.ChangePasswordSuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.ChangePasswordError + e);
        }
    }
}
