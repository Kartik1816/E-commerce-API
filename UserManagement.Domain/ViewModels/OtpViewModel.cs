using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.ViewModels;

public class OtpViewModel
{
    [Required(ErrorMessage = "OTP is required.")]
    [MaxLength(6, ErrorMessage = "OTP cannot exceed 6 digits.")]
    [MinLength(6, ErrorMessage = "OTP must be exactly 6 digits.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit number.")]
    public int OTP { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [Required(ErrorMessage = "Email is required.")]
    public string? Email { get; set; }
}
