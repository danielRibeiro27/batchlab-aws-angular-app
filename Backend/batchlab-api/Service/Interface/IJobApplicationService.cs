using BatchlabApi.Dto;

namespace BatchLabApi.Service.Interface
{
    public interface IJobApplicationService
    {
        Task<bool> CreateAsync(JobDto job);
        JobDto Get(int id);
        IEnumerable<JobDto> GetAll();
        void Update(JobDto job);
        void Delete(int id);
    }
}