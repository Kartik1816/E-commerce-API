namespace UserManagement.Domain.ViewModels;

public class PaymentVerificationRequest
{
    public string? PaymentId { get; set; }

    public string? OrderId { get; set; }

    public string? Signature { get; set; }

    public int OrderModelId { get; set; }
}
