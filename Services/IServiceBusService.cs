using ApiServiceApp.Models;

namespace ApiServiceApp.Services;

public interface IServiceBusService
{
    Task SendMessageAsync(UserDataRequest userData);
}
