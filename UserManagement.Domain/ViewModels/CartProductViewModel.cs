namespace UserManagement.Domain.ViewModels;

public class CartProductViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = null!;
    public decimal Discount { get; set; }
}
