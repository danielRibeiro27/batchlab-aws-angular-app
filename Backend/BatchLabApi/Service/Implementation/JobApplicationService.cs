using BatchLabApi.Dto;
using BatchLabApi.Service.Interface;
using BatchLabApi.Infrastructure.Interface;
using BatchLabApi.Domain;

namespace BatchLabApi.Service.Implementation
{
    public class JobApplicationService(IMessageBus messageBus) : IJobApplicationService
    {
        private readonly IMessageBus _messageBus = messageBus;

        public async Task<bool> CreateAsync(JobEntity job)
        {
            //TO-DO: Save job to database
            return await _messageBus.PublishAsync(job);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public JobEntity Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<JobEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(JobEntity job)
        {
            throw new NotImplementedException();
        }
    }
}