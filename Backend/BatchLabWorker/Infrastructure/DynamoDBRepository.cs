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
                    { "#s", "Status" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":status", new AttributeValue { S = job.Status } }
                },
                UpdateExpression = "SET #s = :status"
            };

            await _dynamoDbClient.UpdateItemAsync(updateItemRequest);
        }
    }
}