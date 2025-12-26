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

        public async Task<bool> SendMessageAsync(string message)
        {
            try
            {
                AmazonSQSClient client = new(Amazon.RegionEndpoint.SAEast1);
                var queueUrl = await client.GetQueueUrlAsync("BatchlabJobs");
                Console.WriteLine(queueUrl.QueueUrl);

                var sendMessageRequest = new SendMessageRequest()
                {
                    QueueUrl = queueUrl.QueueUrl,
                    MessageBody = message
                };
                var response = await client.SendMessageAsync(sendMessageRequest);
                Console.WriteLine($"Message sent with ID: {response.MessageId}");
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }
    }
}