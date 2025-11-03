namespace ApiServiceApp.Models;

/// <summary>
/// Standard API response model
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}
