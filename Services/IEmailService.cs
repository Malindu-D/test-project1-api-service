namespace ApiServiceApp.Services;

public interface IEmailService
{
    Task<bool> TriggerEmailSendingAsync(string receiverEmail);
}
