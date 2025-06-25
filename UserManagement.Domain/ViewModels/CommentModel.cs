namespace UserManagement.Domain.ViewModels;

public class CommentModel
{
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string? UserName { get; set; }

    public int? Rating { get; set; } = 0;
}
