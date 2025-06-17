using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UserManagement.Domain.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Product Name is required")]
    [MaxLength(50, ErrorMessage = "Product name cannot exceed 50 characters.")]
    [MinLength(2,ErrorMessage ="Product name should atleast contains 2 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\- ]{2,}$", ErrorMessage = "Product name must be at least 2 characters long and can only contain letters, numbers, spaces, and hyphens.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public IFormFile? ProductImage { get; set; }
    public decimal Discount { get; set; } = 0;
    public int UserId { get; set; } = 0;
}
