namespace UserManagement.Domain.ViewModels;

public class InventoryItemViewModel
{
    public string? ProductName { get; set; }
    public int TotalStock { get; set; }
    public int SoldQuantity { get; set; }
    public decimal Revenue { get; set; }
    public DateTime LastSoldDate { get; set; }
}
