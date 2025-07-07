using System.Text.RegularExpressions;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Services;

public class ValidationService : IValidationService
{
    public List<ValidationError> errors;

    const string EmailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    const string PasswordRegex = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
    const string PhoneRegex = @"^\d{10}$";
    const string NameRegex = @"^[a-zA-Z]{2,}$";
    const string AddressRegex = @"^[a-zA-Z0-9\s,.'-]{3,}$";

    public void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.EmailRequired,
                reference = "email",
                parameter = "email",
                errorCode = CustomErrorCode.NullEmail
            });
        }
        else if (!Regex.IsMatch(email, EmailRegex))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.InvalidEmailFormat,
                reference = "email",
                parameter = "email",
                errorCode = CustomErrorCode.InvalidEmailFormat
            });
        }
    }
    public void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.PasswordRequired,
                reference = "Password",
                parameter = "Password",
                errorCode = CustomErrorCode.NullPassword
            });
        }
        else if (!Regex.IsMatch(password, PasswordRegex))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.InvalidPasswordFormat,
                reference = "Password",
                parameter = "Password",
                errorCode = CustomErrorCode.InvalidPasswordFormat
            });
        }
    }
    public void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.PhoneRequired,
                reference = "Phone",
                parameter = "Phone",
                errorCode = CustomErrorCode.NullPhone
            });
        }
        else if (!Regex.IsMatch(phone, PhoneRegex))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.InvalidPhoneFormat,
                reference = "Phone",
                parameter = "Phone",
                errorCode = CustomErrorCode.InvalidPhoneFormat
            });
        }
    }
    public void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NameRequired,
                reference = "Name",
                parameter = "Name",
                errorCode = CustomErrorCode.NullName
            });
        }
        else if (!Regex.IsMatch(name, NameRegex))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.InvalidNameFormat,
                reference = "Name",
                parameter = "Name",
                errorCode = CustomErrorCode.InvalidNameFormat
            });
        }
    }
    public void ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.AddressRequired,
                reference = "Address",
                parameter = "Address",
                errorCode = CustomErrorCode.NullAddress
            });
        }
        else if (!Regex.IsMatch(address, AddressRegex))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.InvalidAddressFormat,
                reference = "Address",
                parameter = "Address",
                errorCode = CustomErrorCode.InvalidAddressFormat
            });
        }
    }

    public List<ValidationError> ValidateAuthModel(AuthViewModel authViewModel)
    {
        errors = new List<ValidationError>();

        if (authViewModel == null)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NullModel,
                reference = "AuthViewModel",
                parameter = "AuthViewModel",
                errorCode = CustomErrorCode.NullModel
            });
            return errors;
        }

        ValidateEmail(authViewModel.Email);
        ValidatePassword(authViewModel.Password);
        return errors;
    }

    public List<ValidationError> ValidateRegistrationModel(RegistrationViewModel registrationViewModel)
    {
        errors = new List<ValidationError>();

        if (registrationViewModel == null)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NullModel,
                reference = "RegistrationViewModel",
                parameter = "RegistrationViewModel",
                errorCode = CustomErrorCode.NullModel
            });
            return errors;
        }

        ValidateEmail(registrationViewModel.Email);
        ValidatePassword(registrationViewModel.Password);
        ValidatePhone(registrationViewModel.PhoneNumber);
        ValidateName(registrationViewModel.FirstName);
        ValidateName(registrationViewModel.LastName);
        ValidateAddress(registrationViewModel.Address);

        return errors;
    }
    public List<ValidationError> ValidateChangePasswordModel(ChangePasswordViewModel changePasswordViewModel)
    {
        errors = new List<ValidationError>();

        if (changePasswordViewModel == null)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NullModel,
                reference = "ChangePasswordViewModel",
                parameter = "ChangePasswordViewModel",
                errorCode = CustomErrorCode.NullModel
            });
            return errors;
        }

        ValidatePassword(changePasswordViewModel.OldPassword);
        ValidatePassword(changePasswordViewModel.NewPassword);
        ValidatePassword(changePasswordViewModel.ConfirmPassword);
        if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmPassword)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NewAndConfirmPasswordMismatch,
                reference = "ConfirmPassword",
                parameter = "ConfirmPassword",
                errorCode = CustomErrorCode.NewAndConfirmPasswordMismatch
            });
        }
        return errors;
    }
    public List<ValidationError> ValidateProductModel(ProductViewModel productViewModel)
    {

        errors = new List<ValidationError>();

        if (productViewModel == null)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NullModel,
                reference = "ProductViewModel",
                parameter = "ProductViewModel",
                errorCode = CustomErrorCode.NullModel
            });
            return errors;
        }

        if (string.IsNullOrWhiteSpace(productViewModel.Name))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.ProductNameRequired,
                reference = "Name",
                parameter = "Name",
                errorCode = CustomErrorCode.NullName
            });
        }
        else if (productViewModel.Name.Length < 2 || productViewModel.Name.Length > 50 ||
                 !Regex.IsMatch(productViewModel.Name, @"^[a-zA-Z0-9\- ]{2,}$"))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.InvalidProductNameFormat,
                reference = "Name",
                parameter = "Name",
                errorCode = CustomErrorCode.InvalidNameFormat
            });
        }

        if (string.IsNullOrWhiteSpace(productViewModel.Description))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.ProductDescriptionRequired,
                reference = "Description",
                parameter = "Description",
                errorCode = CustomErrorCode.NullDescription
            });
        }

        if (productViewModel.Price <= 0)
        {
            errors.Add(new ValidationError
            {
                message = "Price must be a positive number.",
                reference = "Price",
                parameter = "Price",
                errorCode = CustomErrorCode.InvalidPrice
            });
        }

        if (productViewModel.Discount < 0 || productViewModel.Discount > 100)
        {
            errors.Add(new ValidationError
            {
                message = "Discount percentage must be between 0 and 100.",
                reference = "Discount",
                parameter = "Discount",
                errorCode = CustomErrorCode.InvalidDiscount
            });
        }

        return errors;
    }
    public List<ValidationError> ValidateEditProfileModel(EditProfileViewModel editProfileViewModel)
    {
        errors = new List<ValidationError>();

        if (editProfileViewModel == null)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NullModel,
                reference = "EditProfileViewModel",
                parameter = "EditProfileViewModel",
                errorCode = CustomErrorCode.NullModel
            });
            return errors;
        }

        ValidateEmail(editProfileViewModel.Email);
        ValidatePhone(editProfileViewModel.PhoneNumber);
        ValidateName(editProfileViewModel.FirstName);
        ValidateName(editProfileViewModel.LastName);
        ValidateAddress(editProfileViewModel.Address);

        return errors;
    }
    public List<ValidationError> ValidateOtpModel(OtpViewModel otpViewModel)
    {
        errors = new List<ValidationError>();

        if (otpViewModel == null)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NullModel,
                reference = "OtpViewModel",
                parameter = "OtpViewModel",
                errorCode = CustomErrorCode.NullModel
            });
            return errors;
        }

        if (otpViewModel.OTP <= 0 || !Regex.IsMatch(otpViewModel.OTP.ToString(), @"^\d{6}$"))
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.OTPInvalid,
                reference = "OTP",
                parameter = "OTP",
                errorCode = CustomErrorCode.OTPInvalid
            });
        }

        ValidateEmail(otpViewModel.Email ?? string.Empty);

        return errors;
    }
    public List<ValidationError> ValidateResetPasswordModel(ResetPasswordViewModel resetPasswordViewModel)
    {
        errors = new List<ValidationError>();

        if (resetPasswordViewModel == null)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NullModel,
                reference = "ResetPasswordViewModel",
                parameter = "ResetPasswordViewModel",
                errorCode = CustomErrorCode.NullModel
            });
            return errors;
        }

        ValidateEmail(resetPasswordViewModel.Email ?? string.Empty);
        ValidatePassword(resetPasswordViewModel.NewPassword);
        ValidatePassword(resetPasswordViewModel.ConfirmPassword);

        if (resetPasswordViewModel.NewPassword != resetPasswordViewModel.ConfirmPassword)
        {
            errors.Add(new ValidationError
            {
                message = CustomErrorMessage.NewAndConfirmPasswordMismatch,
                reference = "ConfirmPassword",
                parameter = "ConfirmPassword",
                errorCode = CustomErrorCode.NewAndConfirmPasswordMismatch
            });
        }

        return errors;
    }
    public List<ValidationError> ValidateUserEmail(string email)
    {
        errors = new List<ValidationError>();
        ValidateEmail(email);
        return errors;
    }
}
