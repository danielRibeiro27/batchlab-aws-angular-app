using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BatchLabApi.Domain;
using BatchLabApi.Infrastructure.Interface;

namespace BatchLabApi.Infrastructure.Implementation{
    public class DynamoDBRepository(AmazonDynamoDBClient dynamoDbClient) : IJobsRepository
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient = dynamoDbClient;  
        private const string TableName = "Jobs";
        public async Task<bool> CreateAsync(JobEntity entity)
        {
            //Used Document model here for simplicity
            var jobAsJson = JsonSerializer.Serialize(entity);
            var itemAsDocument = Document.FromJson(jobAsJson);
            var itemAsAttributes = itemAsDocument.ToAttributeMap();
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = itemAsAttributes
            };

            var response = await _dynamoDbClient.PutItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<List<JobEntity>> GetAllAsync()
        {
            var request = new ScanRequest
            {
                TableName = "Jobs",
                Limit = 25 //TO-DO: Implement batch get item by user id 
            };

            //Used manual mapping here for learning purposes
            var response = await _dynamoDbClient.ScanAsync(request);
            var jobs = new List<JobEntity>(); //TO-DO: Use DynamoDBContext for mapping
            foreach (var item in response.Items)
            {
                var job = new JobEntity
                {
                    Id = Guid.Parse(item["Id"].S),
                    Description = item["Description"].S,
                    Status = item["Status"].S,
                    CreatedAt = DateTime.Parse(item["CreatedAt"].S)
                };
                jobs.Add(job);
            }
            
            return jobs;
        }

        public async Task<JobEntity?> GetByIdAsync(string id)
        {
            var request = new GetItemRequest
            {
                TableName = "Jobs",
                Key = new Dictionary<string, AttributeValue>()
                {
                    {"Id", new AttributeValue{ S = id } }
                }
            };

            var response = await _dynamoDbClient.GetItemAsync(request);
            if (!response.IsItemSet)
            {
                return null;
            }

            //Used manual mapping here for learning purposes
            var item = response.Item;
            var job = new JobEntity
            {
                Id = Guid.Parse(item["Id"].S),
                Description = item["Description"].S,
                Status = item["Status"].S,
                CreatedAt = DateTime.Parse(item["CreatedAt"].S)
            };

            return job; 
        }
    }
}