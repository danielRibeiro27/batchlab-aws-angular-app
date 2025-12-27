using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatchLabApi.Infrastructure.Interface
{
    public interface IJobsRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task CreateAsync(T entity);
    }
}