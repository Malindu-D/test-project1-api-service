using Microsoft.AspNetCore.Mvc;
using ApiServiceApp.Models;
using ApiServiceApp.Services;

namespace ApiServiceApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserDataController : ControllerBase
{
    private readonly IServiceBusService _serviceBusService;
    private readonly ILogger<UserDataController> _logger;

    public UserDataController(IServiceBusService serviceBusService, ILogger<UserDataController> logger)
    {
        _serviceBusService = serviceBusService;
        _logger = logger;
    }

    /// <summary>
    /// Receives user data from Name-Age App and sends to Service Bus
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubmitUserData([FromBody] UserDataRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid data provided",
                    Data = ModelState
                });
            }

            _logger.LogInformation("Received user data: Name={Name}, Age={Age}", request.Name, request.Age);

            // Send to Service Bus
            await _serviceBusService.SendMessageAsync(request);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "User data submitted successfully",
                Data = new { name = request.Name, age = request.Age }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user data");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }
}
