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
    public const string IncorrectCredentials = "INCORRECT_CREDENTIALS";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    public const string RegistrationSuccess = "REGISTRATION_SUCCESS";
    public const string RegistrationError = "REGISTRATION_ERROR";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string CartProductSuccess = "CART_PRODUCT_SUCCESS";
    public const string CartProductError = "CART_PRODUCT_ERROR";
    public const string ProductUserNotFound = "PRODUCT_USER_NOT_FOUND";
    public const string NewAndConfirmPasswordMismatch = "NEW_AND_CONFIRM_PASSWORD_MISMATCH";
    public const string ProductNotFound = "PRODUCT_NOT_FOUND";
    public const string NullDescription = "DESCRIPTION_IS_NULL";
    public const string InvalidPrice = "INVALID_PRICE_FORMAT";
    public const string InvalidDiscount = "INVALID_DISCOUNT_FORMAT";
    public const string OTPInvalid = "OTP_INVALID";
    public const string CategoryNotFound = "CATEGORY_NOT_FOUND";
    public const string NoOfferedProductNotFound = "NO_OFFERED_PRODUCT_NOT_FOUND";
}
