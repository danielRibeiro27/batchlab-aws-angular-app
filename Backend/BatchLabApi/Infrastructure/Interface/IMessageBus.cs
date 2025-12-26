namespace BatchLabApi.Infrastructure.Interface
{
    public interface IMessageBus
    {
        Task<bool> SendMessageAsync(string message);
    }
}