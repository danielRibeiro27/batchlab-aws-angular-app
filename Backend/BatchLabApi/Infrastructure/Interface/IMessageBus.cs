using BatchLabApi.Domain;

namespace BatchLabApi.Infrastructure.Interface
{
    //TO-DO: Generic message bus interface
    public interface IMessageBus
    {
        Task<bool> PublishAsync(JobEntity job);
    }
}