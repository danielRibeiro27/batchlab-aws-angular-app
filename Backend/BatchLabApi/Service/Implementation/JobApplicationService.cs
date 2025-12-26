using BatchLabApi.Dto;
using BatchLabApi.Service.Interface;
using BatchLabApi.Infrastructure.Interface;

namespace BatchLabApi.Service.Implementation
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IMessageBus? _messageBus;
        public JobApplicationService(IMessageBus? messageBus = null)
        {
            _messageBus = messageBus;
        }

        public async Task<bool> CreateAsync(JobDto job)
        {
            Infrastructure.Implementation.SQSMessageBus messageBus = new();
            return await messageBus.PublishAsync(job.Desc);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public JobDto Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<JobDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(JobDto job)
        {
            throw new NotImplementedException();
        }
    }
}