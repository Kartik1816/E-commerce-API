using System.Security.Cryptography.X509Certificates;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IValidationService
{
    public List<ValidationError> ValidateAuthModel(AuthViewModel authViewModel);
    public List<ValidationError> ValidateRegistrationModel(RegistrationViewModel registrationViewModel);
    public List<ValidationError> ValidateChangePasswordModel(ChangePasswordViewModel changePasswordViewModel);
    public List<ValidationError> ValidateProductModel(ProductViewModel productViewModel);
    public List<ValidationError> ValidateEditProfileModel(EditProfileViewModel editProfileViewModel);
    public List<ValidationError> ValidateOtpModel(OtpViewModel otpViewModel);
    public List<ValidationError> ValidateResetPasswordModel(ResetPasswordViewModel resetPasswordViewModel);
    public List<ValidationError> ValidateUserEmail(string email);
}
