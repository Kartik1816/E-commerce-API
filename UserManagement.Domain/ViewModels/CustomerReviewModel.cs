namespace UserManagement.Domain.ViewModels;

public class CustomerReviewModel
{
    public int Rating { get; set; }

    public int ProductId { get; set; }

    public string? Comment { get; set; }
    
    public int UserId { get; set; }
}
