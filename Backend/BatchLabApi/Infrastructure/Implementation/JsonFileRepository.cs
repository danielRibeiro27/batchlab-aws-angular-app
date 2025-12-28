using System.Text.Json;
using BatchLabApi.Infrastructure.Interface;
using BatchLabApi.Domain;

namespace BatchLabApi.Infrastructure.Implementation
{
    public class JsonFileRepository : IJobsRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonFileRepository(string filePath)
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            EnsureFileExists();
        }

        public async Task<JobEntity?> GetByIdAsync(string id)
        {
            var items = await ReadAllAsync();
            return items.Find(item => GetId(item) == id);
        }

        public async Task<List<JobEntity>> GetAllAsync()
        {
            var items = await ReadAllAsync();
            return items;
        }

        public async Task<bool> CreateAsync(JobEntity entity)
        {
            var items = await ReadAllAsync();
            items.Add(entity);
            await WriteAllAsync(items);
            return true;
        }

        // TODO: Add thread-safety mechanisms (e.g., locking) to protect concurrent read/write operations
        // and prevent race conditions or data corruption in multi-threaded environments.
        private async Task<List<JobEntity>> ReadAllAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return string.IsNullOrWhiteSpace(json) 
                ? new List<JobEntity>() 
                : JsonSerializer.Deserialize<List<JobEntity>>(json) ?? new List<JobEntity>();
        }

        private async Task WriteAllAsync(List<JobEntity> items)
        {
            var json = JsonSerializer.Serialize(items, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }

        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        // TODO: This method uses reflection to access the "Id" property, which is fragile and could cause runtime exceptions
        // if the type T doesn't have an "Id" property or if it has a different casing.
        // Consider using a generic constraint or interface to enforce the presence of an Id property.
        private static string GetId(JobEntity item)
        {
            var idProperty = typeof(JobEntity).GetProperty("Id");
            return idProperty?.GetValue(item)?.ToString() ?? string.Empty;
        }
    }
}