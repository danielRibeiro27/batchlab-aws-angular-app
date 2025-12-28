using BatchLabApi.Domain;

namespace BatchLabApi.Infrastructure.Interface
{
    public interface IJobsRepository
    {
        Task<JobEntity?> GetByIdAsync(string id);
        Task<bool> CreateAsync(JobEntity entity);
        Task<List<JobEntity>> GetAllAsync();
    }
}