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
var _repository = new DynamoDBRepository(new Amazon.DynamoDBv2.AmazonDynamoDBClient());
Console.WriteLine("Worker Queue: " + queueUrl.QueueUrl);

if(queueUrl == null || string.IsNullOrEmpty(queueUrl.QueueUrl))
    throw new NullReferenceException("Queue URL is null or empty. Please check your AWS SQS configuration.");

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
            ArgumentException.ThrowIfNullOrEmpty(message.Body, nameof(message.Body));
            Console.WriteLine("Message received: " + message.Body);

            try
            {
                JobEntity jobEntity = JsonSerializer.Deserialize<JobEntity>(message.Body)!; //throw serialize exception

                string status = await _repository.GetJobStatusAsync(jobEntity.Id.ToString()); //throw repo exception
                if(status != "Pending") //idempotency
                    continue;

                await FakeProcessJobAsync(jobEntity); //throw processing exception       

                jobEntity.Status = "Completed";

                // var _repository = new JsonFileRepository("../BatchLabApi/jobs.json");
                // _ = await _repository.UpdateAsync(jobEntity);
                await _repository.UpdateJobStatusAsync(jobEntity); //throw ConditionalCheckFailedException
                Console.WriteLine("Job updated in repository: " + jobEntity.Id);

                var deleteMessageRequest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl.QueueUrl,
                    ReceiptHandle = message.ReceiptHandle
                };
                await client.DeleteMessageAsync(deleteMessageRequest); //throw ReceiptHandleIsInvalid 
                Console.WriteLine("Message deleted: " + message.MessageId);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize message {message.MessageId}: {ex.Message}");
            }
            catch (Amazon.DynamoDBv2.Model.ConditionalCheckFailedException ex)
            {
                Console.WriteLine($"Failed to update job status - conditional check failed for job {message.MessageId}: {ex.Message}");
            }
            catch (ReceiptHandleIsInvalidException ex)
            {
                Console.WriteLine($"Failed to delete message - invalid receipt handle for message {message.MessageId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message {message.MessageId}: {ex.Message}");
            }
            finally
            {
                //TO-DO: Change status here to failed or completed based on the exception type and retry logic
                Console.WriteLine($"Finished processing message {message.MessageId}");
            }
        }
    }
}

//TO-DO: Add transactional idempotency for preventing server states changes on duplicate
static async Task FakeProcessJobAsync(JobEntity jobData)
{
    //Simulate job processing time
    Random rand = Random.Shared;
    int processingTime = rand.Next(1000, 50000); //Random processing time between 1-50 seconds
    await Task.Delay(processingTime);
    if(rand.NextDouble() < 0.5) //50% chance to simulate job failure
    {
        throw new Exception("Simulated job processing failure.");
    }

    Console.WriteLine($"Processed job: {jobData} in {processingTime} ms");
}