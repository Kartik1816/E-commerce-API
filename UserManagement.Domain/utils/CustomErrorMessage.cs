namespace UserManagement.Domain.utils;

public class CustomErrorMessage
{
    public const string NullModel = "Model cannot be null.";
    public const string EmailRequired = "Email is required.";
    public const string InvalidEmailFormat = "Invalid email format.";
    public const string PasswordRequired = "Password is required.";
    public const string InvalidPasswordFormat = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
    public const string PhoneRequired = "Phone number is required.";
    public const string InvalidPhoneFormat = "Phone number must be 10 digits long.";
    public const string NameRequired = "Name is required.";
    public const string InvalidNameFormat = "Name must be at least 2 characters long and contain only letters.";
    public const string AddressRequired = "Address is required.";
    public const string InvalidAddressFormat = "Address must be at least 3 characters long and can include letters, numbers, spaces, commas, periods, and hyphens.";
    public const string InvalidAuthModel = "Authentication model is invalid.";
    public const string UserNotRegistered = "User not registered.";
    public const string LoginSuccess = "Login successful.";
}
