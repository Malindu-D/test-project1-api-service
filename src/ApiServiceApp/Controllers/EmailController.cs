using Microsoft.AspNetCore.Mvc;
using ApiServiceApp.Models;
using ApiServiceApp.Services;

namespace ApiServiceApp.Controllers;

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IEmailService emailService, ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Triggers email sending via Java Email Service
    /// Called by Email Notification App with receiver email address
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendEmails([FromBody] EmailRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ReceiverEmail))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Receiver email is required"
                });
            }

            _logger.LogInformation("Email send request received for: {Email}", request.ReceiverEmail);

            var result = await _emailService.TriggerEmailSendingAsync(request.ReceiverEmail);

            if (result)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"Email sent successfully to {request.ReceiverEmail}"
                });
            }
            else
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Failed to send email"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while sending email"
            });
        }
    }
}
