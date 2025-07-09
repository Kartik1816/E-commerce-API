namespace UserManagement.Domain.ViewModels;

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
