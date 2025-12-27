using BatchLabApi.Domain;
using BatchLabApi.Dto;

namespace BatchLabApi.Service.Interface
{
    public interface IJobApplicationService
    {
        Task<bool> CreateAsync(JobEntity job);
        JobEntity Get(int id);
        IEnumerable<JobEntity> GetAll();
    }
}