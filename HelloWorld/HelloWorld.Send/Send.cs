using System;
using RabbitMQ.Client;
using System.Text;

namespace HelloWorld.Send
{
    class Send
    {
        static void Main(string[] args)
        {
            string message;
            do
            {
                Console.Write("Type the message to be sent: ");
                message = Console.ReadLine();
                SendToRabbitMQ(message);
                Console.WriteLine(" Write [exit] to quit.");
            } while (message.Trim().ToLower() != "exit");

        }

        private static void SendToRabbitMQ(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
        }
    }
}
