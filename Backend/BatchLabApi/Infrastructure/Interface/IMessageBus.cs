namespace BatchLabApi.Infrastructure.Interface
{
    public interface IMessageBus
    {
        Task<bool> PublishAsync(string message);
    }
}