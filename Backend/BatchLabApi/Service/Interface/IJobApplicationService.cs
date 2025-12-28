using BatchLabApi.Domain;
using BatchLabApi.Dto;

namespace BatchLabApi.Service.Interface
{
    public interface IJobApplicationService
    {
        Task<bool> PublishAsync(JobEntity job);
        Task<JobEntity?> GetByIdAsync(string id);
        Task<List<JobEntity>> GetAllAsync();
    }
}