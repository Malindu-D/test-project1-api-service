namespace ApiServiceApp.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailService> _logger;
    private readonly string _emailServiceUrl;

    public EmailService(HttpClient httpClient, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Get Email Export Service URL from environment variable or appsettings
        // Support both EMAIL_EXPORT_SERVICE_URL and JAVA_EMAIL_SERVICE_URL for flexibility
        _emailServiceUrl = Environment.GetEnvironmentVariable("EMAIL_EXPORT_SERVICE_URL")
            ?? Environment.GetEnvironmentVariable("JAVA_EMAIL_SERVICE_URL")
            ?? configuration["EmailExportService:BaseUrl"]
            ?? string.Empty;

        // Ensure URL has https:// prefix if not empty
        if (!string.IsNullOrEmpty(_emailServiceUrl) && !_emailServiceUrl.StartsWith("http://") && !_emailServiceUrl.StartsWith("https://"))
        {
            _emailServiceUrl = $"https://{_emailServiceUrl}";
            _logger.LogInformation("Added https:// prefix to Email Service URL: {Url}", _emailServiceUrl);
        }

        if (string.IsNullOrEmpty(_emailServiceUrl))
        {
            _logger.LogWarning("Email Export Service URL is not configured");
        }
        else
        {
            _logger.LogInformation("Email Export Service configured at: {Url}", _emailServiceUrl);
        }
    }

    public async Task<bool> TriggerEmailSendingAsync(string receiverEmail)
    {
        try
        {
            if (string.IsNullOrEmpty(_emailServiceUrl))
            {
                throw new InvalidOperationException("Email Export Service URL is not configured");
            }

            // Call Email Export Service endpoint to send emails
            var endpoint = $"{_emailServiceUrl}/api/email/send";
            _logger.LogInformation("Calling Email Export Service at: {Endpoint} for receiver: {Email}", endpoint, receiverEmail);

            var requestBody = new { receiverEmail };
            var response = await _httpClient.PostAsJsonAsync(endpoint, requestBody);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully triggered email sending to {Email}", receiverEmail);
                return true;
            }
            else
            {
                _logger.LogWarning("Email Export Service returned status code: {StatusCode}", response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Email Export Service");
            return false;
        }
    }
}
