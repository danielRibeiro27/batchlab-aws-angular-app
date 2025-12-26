namespace BatchlabApi.Service.Interface
{
    public interface IMessageBus
    {
        Task<bool> SendMessageAsync(string message);
    }
}