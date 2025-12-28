using BatchLabApi.Dto;
using BatchLabApi.Service.Interface;
using BatchLabApi.Infrastructure.Interface;
using BatchLabApi.Domain;

namespace BatchLabApi.Service.Implementation
{
    public class JobApplicationService(IMessageBus messageBus, IJobsRepository jobRepository) : IJobApplicationService
    {
        private readonly IMessageBus _messageBus = messageBus;
        private readonly IJobsRepository _jobsRepository = jobRepository;

        public async Task<bool> PublishAsync(JobEntity job)
        {
            //No need to await here as we want to do both concurrently
            await _messageBus.PublishAsync(job);
            await _jobsRepository.CreateAsync(job);
            return true;
        }

        public async Task<JobEntity?> GetByIdAsync(string id)
        {
            JobEntity? job = await _jobsRepository.GetByIdAsync(id);
            return job;
        }

        public async Task<List<JobEntity>> GetAllAsync()
        {
            return await _jobsRepository.GetAllAsync();
        }
    }
}