using System.Runtime.CompilerServices;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using BatchLabWorker.Domain;
using BatchLabWorker.Infrastructure;

//TO-DO: Move AWS config to appsettings.json
//TO-DO: Use interface for AWS client?
var client = new AmazonSQSClient();
var queueUrl = await client.GetQueueUrlAsync("BatchlabJobs");
Console.WriteLine("Worker Queue: " + queueUrl.QueueUrl);

//TO-DO: Move receive message request config to appsettings.json
var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrl.QueueUrl,
    MaxNumberOfMessages = 1,
    WaitTimeSeconds = 20
};

//TO-DO: Graceful shutdown
while(true){
    var response = await client.ReceiveMessageAsync(receiveMessageRequest);
    if(response != null && response.Messages?.Count > 0 && response.HttpStatusCode == System.Net.HttpStatusCode.OK)
    {
        foreach(var message in response.Messages)
        {
            //TO-DO: Add error handling (try-catch) around message processing and deletion to prevent message loss and worker crashes
            Console.WriteLine("Message received: " + message.Body); //TO-DO: Process the message (e.g., perform the job)

            JobEntity jobEntity = JsonSerializer.Deserialize<JobEntity>(message.Body)!;
            FakeProcessJob(jobEntity);
            jobEntity.Status = "Completed";

            var _repository = new JsonFileRepository("../BatchLabApi/jobs.json");
            var existingJob = await _repository.GetByIdAsync(jobEntity.Id.ToString());
            if(existingJob == null)
            {
                // Job should have been created by the API before being enqueued
                // If it doesn't exist, this indicates a race condition or data inconsistency
                Console.WriteLine($"Warning: Job {jobEntity.Id} not found in repository. Skipping processing.");
                // Delete the message to prevent reprocessing
                var deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl.QueueUrl,
                    ReceiptHandle = message.ReceiptHandle
                };
                await client.DeleteMessageAsync(deleteRequest);
                Console.WriteLine("Message deleted: " + message.MessageId);
                continue;
            }
            
            // Update the existing job with the completed status
            _ = await _repository.UpdateAsync(jobEntity);
            Console.WriteLine("Job updated in repository: " + jobEntity.Id);


            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl.QueueUrl,
                ReceiptHandle = message.ReceiptHandle
            };
            await client.DeleteMessageAsync(deleteMessageRequest);
            Console.WriteLine("Message deleted: " + message.MessageId);
        }
    }
}

static void FakeProcessJob(JobEntity jobData)
{
    //Simulate job processing time
    Random rand = new Random();
    int processingTime = rand.Next(1000, 50000); //Random processing time between 1-50 seconds
    System.Threading.Thread.Sleep(processingTime);
    Console.WriteLine($"Processed job: {jobData} in {processingTime} ms");

    if(rand.NextDouble() < 0.1) //10% chance to simulate job failure
    {
        throw new Exception("Simulated job processing failure.");
    }
}