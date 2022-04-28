using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace QueueRecieverApp
{
    class Program
    {
        //Connection String
        static string connectionString = "Endpoint=sb://mysericebusdemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vtJBjAIb9vktHlfyvbIO5GHzCsGeebGHKDf5ioOZPT0=";
        //name of our Service Bus Queue
        static string queueName = "myqueue";
        //the Client that owns the connection and can be used to create senders reciever
        static ServiceBusClient client;
        //the Processor that reads and process the message from the Queue
        static ServiceBusProcessor processor;


        //Method1: For Handling Recieved messages and Displaying them 
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Recieved: {body}");
            //Complete the message. message is deleted from the queue.
            await args.CompleteMessageAsync(args.Message);
        }


        //Method2: Handling any error when recieveing messages and Displaying them
         static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Demo for Receiving Message from Service Bus..");

            //Creating a Client Object so that we can create Sender and reciever object
            client = new ServiceBusClient(connectionString);

            //Creating a prcoessor that can be used to process the messages 
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());


            try
            {
                //Add Handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                //Add Handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;
                //Start processing 
                await processor.StartProcessingAsync();
                Console.WriteLine("Wait for a Minute and THEN PRESS ANY KEY TO END the Processing  :-) ");
                Console.ReadKey();

                //Stop processing
                Console.WriteLine("\n Stopping the Receiver..... ");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped recieving messages....!!! ");
            }
            
            finally
            {
                //Calling disposeAsync on clients types is required to ensure that objects are properly cleaned up.
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
