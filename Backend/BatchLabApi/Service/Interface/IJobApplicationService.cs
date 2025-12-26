using BatchLabApi.Dto;

namespace BatchLabApi.Service.Interface
{
    public interface IJobApplicationService
    {
        Task<bool> CreateAsync(JobDto job);
        JobDto Get(int id);
        IEnumerable<JobDto> GetAll();
    }
}