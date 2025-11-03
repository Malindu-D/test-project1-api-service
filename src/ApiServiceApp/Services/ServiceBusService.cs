using Azure.Messaging.ServiceBus;
using ApiServiceApp.Models;
using System.Text.Json;

namespace ApiServiceApp.Services;

public class ServiceBusService : IServiceBusService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<ServiceBusService> _logger;
    private readonly string _queueName;

    public ServiceBusService(IConfiguration configuration, ILogger<ServiceBusService> logger)
    {
        _logger = logger;
        
        // Get connection string from environment variable or appsettings
        var connectionString = Environment.GetEnvironmentVariable("AZURE_SERVICEBUS_CONNECTIONSTRING")
            ?? configuration["AzureServiceBus:ConnectionString"];
        
        _queueName = Environment.GetEnvironmentVariable("AZURE_SERVICEBUS_QUEUENAME")
            ?? configuration["AzureServiceBus:QueueName"] 
            ?? "userdata-queue";

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Service Bus connection string is not configured");
        }

        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(_queueName);
    }

    public async Task SendMessageAsync(UserDataRequest userData)
    {
        try
        {
            var messageBody = JsonSerializer.Serialize(userData);
            var message = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json",
                Subject = "UserData"
            };

            await _sender.SendMessageAsync(message);
            _logger.LogInformation("Message sent to Service Bus queue: {QueueName}", _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to Service Bus");
            throw;
        }
    }
}
