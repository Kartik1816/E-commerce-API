using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.ViewModels;

public class OtpViewModel
{
    [Required(ErrorMessage = "OTP is required.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit number.")]
    public int OTP { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [Required(ErrorMessage = "Email is required.")]
    public string? Email { get; set; }
}
