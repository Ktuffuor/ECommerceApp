namespace Common.CommonResponse;

public class ApiResponse<T>
{
    public bool Success { get; set; } 
    public int StatusCode { get; set; }
    public string Message { get; set; } = "Success";
    public T? Data { get; set; }
}