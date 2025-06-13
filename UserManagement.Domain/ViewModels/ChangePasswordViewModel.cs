using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "OldPassword is required.")]
    [MinLength(8, ErrorMessage = "OldPassword must be at least 6 characters.")]
    [MaxLength(50, ErrorMessage = "OldPassword cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Please Enter correct OldPassword")]
    public string OldPassword { get; set; } = null!;

    [Required(ErrorMessage = "NewPassword is required.")]
    [MinLength(8, ErrorMessage = "NewPassword must be at least 6 characters.")]
    [MaxLength(50, ErrorMessage = "NewPassword cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Please Enter correct NewPassword")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password is required.")]
    [MinLength(8, ErrorMessage = "Confirm Password must be at least 6 characters.")]
    [MaxLength(50, ErrorMessage = "Confirm Password cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Please Enter correct Confirm Password")]
    [Compare("NewPassword", ErrorMessage = "Confirm Password does not match with New Password.")]
    public string ConfirmPassword { get; set; } = null!;
    public int Id { get; set; }
}
