using BatchlabApi.Dto;
using BatchlabApi.Service.Implementation;
using BatchlabApi.Service.Interface;
using BatchLabApi.Service.Interface;

namespace BatchlabApi.Service.Implementation
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IMessageBus _messageBus;
        public JobApplicationService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task<bool> CreateAsync(JobDto job)
        {
            SQSMessageBus messageBus = new();
            return await messageBus.SendMessageAsync("HELLO_WORD_MESSAGE");
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