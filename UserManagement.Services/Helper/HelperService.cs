namespace UserManagement.Services.Helper;

public class HelperService
{
    public static int GenerateSixDigitOTP()
    {
        Random random = new Random();
        int otp = random.Next(100000, 999999);
        return otp;
    }
}
