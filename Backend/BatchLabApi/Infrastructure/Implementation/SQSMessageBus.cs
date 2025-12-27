using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using BatchLabApi.Domain;
using BatchLabApi.Infrastructure.Interface;

namespace BatchLabApi.Infrastructure.Implementation
{
    public class SQSMessageBus() : IMessageBus
    {
        public async Task<bool> PublishAsync(JobEntity job)
        {
            try
            {
                //TO-DO: Move AWS config to appsettings.json
                //TO-DO: Use interface for AWS client?
                //TO-DO: Dispose AmazonSQSClient properly (implement using statement or IDisposable pattern)
                AmazonSQSClient client = new(Amazon.RegionEndpoint.SAEast1);
                var queueUrl = await client.GetQueueUrlAsync("BatchlabJobs"); 
                Console.WriteLine("Queue: " + queueUrl.QueueUrl);

                string jobJson = JsonSerializer.Serialize(job);

                var sendMessageRequest = new SendMessageRequest()
                {
                    QueueUrl = queueUrl.QueueUrl,
                    MessageBody = jobJson
                    //TO-DO: Add message attributes if needed
                };
                var response = await client.SendMessageAsync(sendMessageRequest);
                Console.WriteLine("Message sent with ID: " + response.MessageId);
                return response?.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex) //TO-DO: Handle exceptions properly
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }
    }
}