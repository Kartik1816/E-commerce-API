namespace UserManagement.Domain.ViewModels;

public class TokenViewModel
{
    public string JwtToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
