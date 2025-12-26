using System.Text.Json;
using BatchLabApi.Infrastructure.Interface;

namespace BatchLabApi.Infrastructure.Implementation
{
    public class JsonFileRepository<T> : IJobsRepository<T> where T : class
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonFileRepository(string filePath)
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            EnsureFileExists();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var items = await ReadAllAsync();
            return items.Find(item => GetId(item) == id);
        }

        public async Task CreateAsync(T entity)
        {
            var items = await ReadAllAsync();
            items.Add(entity);
            await WriteAllAsync(items);
        }

        private async Task<List<T>> ReadAllAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return string.IsNullOrWhiteSpace(json) 
                ? new List<T>() 
                : JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        private async Task WriteAllAsync(List<T> items)
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

        private static string GetId(T item)
        {
            var idProperty = typeof(T).GetProperty("Id");
            return idProperty?.GetValue(item)?.ToString() ?? string.Empty;
        }
    }
}