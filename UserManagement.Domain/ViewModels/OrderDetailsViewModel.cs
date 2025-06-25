namespace UserManagement.Domain.ViewModels;

public class OrderDetailsViewModel
{
    public int OrderId { get; set; }

    public string Status { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderProductViewModel> OrderProductViewModels { get; set; } = null!;
}
