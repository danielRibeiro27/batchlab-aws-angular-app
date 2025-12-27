using Amazon.SQS;
using Amazon.SQS.Model;

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
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl.QueueUrl,
                ReceiptHandle = message.ReceiptHandle
            };
            await client.DeleteMessageAsync(deleteMessageRequest);

            //TO-DO: Update job status in database
            Console.WriteLine("Message deleted: " + message.MessageId);
        }
    }
}