using Amazon.SQS;
using Amazon.SQS.Model;

var client = new AmazonSQSClient();
var queueUrl = await client.GetQueueUrlAsync("BatchlabJobs");
Console.WriteLine("Worker Queue: " + queueUrl.QueueUrl);

var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrl.QueueUrl,
    MaxNumberOfMessages = 1,
    WaitTimeSeconds = 20
};

while(true){
    var response = await client.ReceiveMessageAsync(receiveMessageRequest);
    if(response.Messages.Count > 0 && response.HttpStatusCode == System.Net.HttpStatusCode.OK)
    {
        foreach(var message in response.Messages)
        {
            Console.WriteLine("Message received: " + message.Body);
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl.QueueUrl,
                ReceiptHandle = message.ReceiptHandle
            };
            await client.DeleteMessageAsync(deleteMessageRequest);
        }
    }
}