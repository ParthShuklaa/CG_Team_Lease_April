using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;//For Sending values to Service Bus Queueu

namespace QueueSender
{
    class Program
    {

        /*
         * Connection String for your service bus request
         * Name of the Service Bus Queue
         * Client that owns the connection and can be used to create sender and reciever 
         * The sender used to publish the message to the queue
         * Number of message to be sent to the queue
          */
        static string connectionstring = "Endpoint=sb://mysericebusdemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vtJBjAIb9vktHlfyvbIO5GHzCsGeebGHKDf5ioOZPT0=";
        static string queuename = "myqueue";
        static ServiceBusClient client;
        static ServiceBusSender sender;
        private const int NumofMessages = 5;


        static async Task Main(string[] args)
        {
            Console.WriteLine("Weocome to the demo on implementing Service Bus Queue ");
            //Creating Client Object 
            client = new ServiceBusClient(connectionstring);
            sender = client.CreateSender(queuename);

            //Creating a Batch
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            for (int i = 1; i < NumofMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($" The Message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                //use the producer client to send the batch of messages to the Service Bus Queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A Batch of {NumofMessages} messages has been published to the queue");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Caught"+e.Message);
                throw;
            }
            finally
            {
                //Calling DisposeAsync on a client Tyoe is required to ensure that network resource and other 
                // unmanaged objects are propoerly cleaned up
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
            Console.WriteLine("Press any key to end the Application");

        }
    }
}
