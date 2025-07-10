using Microsoft.AspNetCore.Http;

namespace UserManagement.Domain.ViewModels;

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsReleased { get; set; } = false;
    public IFormFile? Image { get; set; }
    public string? ImageUrl { get; set; } 
}
