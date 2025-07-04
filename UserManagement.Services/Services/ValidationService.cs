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
}
