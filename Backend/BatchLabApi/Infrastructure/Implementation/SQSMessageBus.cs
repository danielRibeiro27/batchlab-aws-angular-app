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
            ArgumentNullException.ThrowIfNull(job, nameof(job));
            ArgumentException.ThrowIfNullOrEmpty(job.Id.ToString(), nameof(job.Id));

            //TO-DO: Move AWS config to appsettings.json
            //TO-DO: Use interface for AWS client?
            //TO-DO: Dispose AmazonSQSClient properly (implement using statement or IDisposable pattern)
            AmazonSQSClient client = new(Amazon.RegionEndpoint.SAEast1);
            var queueUrl = await client.GetQueueUrlAsync("BatchlabJobs"); 

            if(queueUrl == null || string.IsNullOrEmpty(queueUrl.QueueUrl))
                throw new NullReferenceException("Queue URL is null or empty. Please check your AWS SQS configuration.");

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
    }
}