using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RPC.RPCServer
{
    class RPCServer
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
                    Console.WriteLine(" [x] Awaiting RPC requests");

                    consumer.Received += (model, ea) =>
                    {
                        string response = null;

                        var body = ea.Body.ToArray();
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;

                        try
                        {
                            var message = Encoding.UTF8.GetString(body);
                            int n = int.Parse(message);
                            Console.WriteLine(" [.] fib({0})", message);
                            response = _fibonacci(n).ToString();
                        }
                        catch (Exception e)
                        {

                            Console.WriteLine(" [.] " + e.Message);
                            response = "";
                        }
                        finally
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(
                                exchange: "", 
                                routingKey: props.ReplyTo, 
                                basicProperties: replyProps, 
                                body: responseBytes);
                            
                            channel.BasicAck(
                                deliveryTag: ea.DeliveryTag, 
                                multiple: false);
                        }

                    };

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }

            }

        }

        /// <summary>
        /// Assumes only valid positive integer input.
        /// Don't expect this one to work for big numbers, and it's probably the slowest recursive implementation possible.
        /// </summary>
        private static int _fibonacci(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return _fibonacci(n - 1) + _fibonacci(n - 2);
        }
    }
}
