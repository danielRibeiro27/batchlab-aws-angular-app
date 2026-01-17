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
            ArgumentNullException.ThrowIfNull(job, nameof(job));
            ArgumentException.ThrowIfNullOrEmpty(job.Id.ToString(), nameof(job.Id));

            _messageBus.PublishAsync(job);
            await _jobsRepository.CreateAsync(job); //only awaits for informing user of db creation
            return true;
        }

        public async Task<JobEntity?> GetByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));
            JobEntity? job = await _jobsRepository.GetByIdAsync(id);
            return job;
        }

        public async Task<List<JobEntity>> GetAllAsync()
        {
            var jobs = await _jobsRepository.GetAllAsync();
            return jobs;
        }
    }
}