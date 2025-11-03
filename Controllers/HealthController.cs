using Microsoft.AspNetCore.Mvc;
using ApiServiceApp.Models;

namespace ApiServiceApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "API Service is running",
            Data = new
            {
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            }
        });
    }
}
