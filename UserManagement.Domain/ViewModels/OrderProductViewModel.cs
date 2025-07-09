namespace UserManagement.Domain.ViewModels;

public class OrderProductViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Discount { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string UniqueCode { get; set; } = null!;
}
