using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PRODUCER - Start");

            var users = new List<long>()
            {
                1, 2, 3, 4, 5
            };

            var random = new Random();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    foreach (var item in users)
                    {
                        channel.QueueDeclare(queue: $"user_{item}",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                    }

                    IBasicProperties props = channel.CreateBasicProperties();

                    var total = 500;
                    for (int i = 1; i <= total; i++)
                    {
                        string message = $"{DateTime.Now}";
                        var user_id = random.Next(1, users.Count + 1);
                        props.Expiration = random.Next(2) == 0 ? null : "120000";

                        SendMEssage(message, channel, user_id, props);

                        System.Threading.Thread.Sleep(1000);
                    }

                }
            }

            Console.WriteLine("PRODUCER - End");
        }

        private static void SendMEssage(string message, IModel channel, int user_id, IBasicProperties props)
        {
            channel.BasicPublish(
                exchange: "",
                routingKey: $"user_{user_id}",
                basicProperties: props,
                body: Encoding.UTF8.GetBytes($"{message}; MaxTimeExisting: {props.Expiration}"));
            Console.WriteLine($" [x] Sent {message} to user_{user_id}.");
        }
    }
}
