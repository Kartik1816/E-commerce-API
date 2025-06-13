using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.ViewModels;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 6 characters.")]
    [MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Please Enter correct Password")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password is required.")]
    [MinLength(8, ErrorMessage = "Confirm Password must be at least 6 characters.")]
    [MaxLength(50, ErrorMessage = "Confirm Password cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Please Enter correct Confirm Password")]
    [Compare("NewPassword", ErrorMessage = "Confirm Password does not match with New Password.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string? Email { get; set; }
}
