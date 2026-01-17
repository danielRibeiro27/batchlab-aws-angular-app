using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BatchLabWorker.Domain;
using System.Threading.Tasks;

namespace BatchLabWorker.Infrastructure
{
    public class DynamoDBRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private const string TableName = "Jobs";

        public DynamoDBRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task UpdateJobStatusAsync(JobEntity job)
        {
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = job.Id.ToString() } }
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#S", "Status" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":status", new AttributeValue { S = job.Status } }
                },
                ConditionExpression = "#S = 'Pending'", // Only update if status is 'Pending'
                UpdateExpression = "SET #S = :status"
            };

            await _dynamoDbClient.UpdateItemAsync(updateItemRequest);
        }
    
        public async Task<string> GetJobStatusAsync(string jobId)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = jobId } }
                }
            };

            var response = await _dynamoDbClient.GetItemAsync(getItemRequest);
            if (response.Item != null && response.Item.ContainsKey("Status"))
            {
                return response.Item["Status"].S;
            }

            throw new Exception($"Job with Id {jobId} not found.");
        }
    }
}