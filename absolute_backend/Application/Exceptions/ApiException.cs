namespace Application.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string message { get; set; } = string.Empty;
    public object? Details { get; }

    public ApiException(int statusCode, string message, object? details = null) : base(message)
    {
        StatusCode = statusCode;
        Details = details;
    }
}