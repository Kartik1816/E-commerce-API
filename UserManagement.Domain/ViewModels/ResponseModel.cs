namespace UserManagement.Domain.ViewModels;

public class ResponseModel
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public Object? Data { get; set; }
    public string? ErrorCode { get; set; }
}
