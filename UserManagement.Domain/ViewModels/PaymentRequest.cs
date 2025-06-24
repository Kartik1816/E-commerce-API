namespace UserManagement.Domain.ViewModels;

public class PaymentRequest
{
    public decimal Amount { get; set; }
    
    public string Currency { get; set; } = "INR";

    public int UserId { get; set; }
}
