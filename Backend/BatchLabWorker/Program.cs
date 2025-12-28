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
            await FakeProcessJobAsync(jobEntity);
            jobEntity.Status = "Completed";

            // var _repository = new JsonFileRepository("../BatchLabApi/jobs.json");
            // _ = await _repository.UpdateAsync(jobEntity);
            var _repository = new DynamoDBRepository(new Amazon.DynamoDBv2.AmazonDynamoDBClient());
            await _repository.UpdateJobStatusAsync(jobEntity);
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

static async Task FakeProcessJobAsync(JobEntity jobData)
{
    //Simulate job processing time
    // TODO: Use Random.Shared instead of creating new Random instance for better performance and thread safety
    Random rand = Random.Shared;
    int processingTime = rand.Next(1000, 50000); //Random processing time between 1-50 seconds
    // TODO: Replace Thread.Sleep with await Task.Delay() to properly support async operations
    await Task.Delay(processingTime);
    if(rand.NextDouble() < 0.1) //10% chance to simulate job failure
    {
        throw new Exception("Simulated job processing failure.");
    }

    Console.WriteLine($"Processed job: {jobData} in {processingTime} ms");
}