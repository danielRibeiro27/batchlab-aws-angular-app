using Amazon.SQS;
using Amazon.SQS.Model;
using BatchLabApi.Infrastructure.Interface;

namespace BatchLabApi.Infrastructure.Implementation
{
    public class SQSMessageBus : IMessageBus
    {
        public SQSMessageBus()
        {
        }

        public async Task<bool> PublishAsync(Dto.JobDto jobDto)
        {
            try
            {
                //TO-DO: Move AWS config to appsettings.json
                //TO-DO: Use interface for AWS client?
                //TO-DO: AmazonSQSClient should be created once and reused (singleton pattern) as it's thread-safe. Creating a new instance per message is inefficient and can lead to resource exhaustion.
                AmazonSQSClient client = new(Amazon.RegionEndpoint.SAEast1);
                var queueUrl = await client.GetQueueUrlAsync("BatchlabJobs"); 
                Console.WriteLine("Queue: " + queueUrl.QueueUrl);

                var sendMessageRequest = new SendMessageRequest()
                {
                    QueueUrl = queueUrl.QueueUrl,
                    MessageBody = jobDto.Desc //TO-DO: Serialize full dto
                    //TO-DO: Add message attributes if needed
                };
                var response = await client.SendMessageAsync(sendMessageRequest);
                Console.WriteLine("Message sent with ID: " + response.MessageId);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex) //TO-DO: Handle exceptions properly
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }
    }
}