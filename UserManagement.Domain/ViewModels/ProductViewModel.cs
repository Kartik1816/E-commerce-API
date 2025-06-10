using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UserManagement.Domain.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50, ErrorMessage = "Product name cannot exceed 50 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Product name can only contain letters and spaces")]
    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Rate { get; set; }

    public IFormFile? ProductImage { get; set; }
    public string? ImagePath { get; set; }
    public string? Description{ get; set; }
}
