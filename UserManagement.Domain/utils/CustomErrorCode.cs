namespace UserManagement.Domain.utils;

public class CustomErrorCode
{
    public const string NullModel = "MODEL_IS_NULL";
    public const string NullEmail = "EMAIL_IS_NULL";
    public const string InvalidEmailFormat = "INVALID_EMAIL_FORMAT";
    public const string NullPassword = "PASSWORD_IS_NULL";
    public const string InvalidPasswordFormat = "INVALID_PASSWORD_FORMAT";
    public const string NullPhone = "PHONE_IS_NULL";
    public const string InvalidPhoneFormat = "INVALID_PHONE_FORMAT";
    public const string NullName = "NAME_IS_NULL";
    public const string InvalidNameFormat = "INVALID_NAME_FORMAT";
    public const string NullAddress = "ADDRESS_IS_NULL";
    public const string InvalidAddressFormat = "INVALID_ADDRESS_FORMAT";
    public const string IsValid = "MODEL_IS_INVALID";
    public const string UserNotRegistered = "USER_NOT_REGISTERED";
}
