using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var consumerId = random.Next(1,6);

            Console.WriteLine($"CONSUMER 'user_{consumerId}' - Start");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: $"user_{consumerId}",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] user_{consumerId}: Received {message} RecevedAt: {DateTime.Now}");
                };
                channel.BasicConsume(queue: $"user_{consumerId}",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

            Console.WriteLine($"CONSUMER 'user_{consumerId}' - End");
        }
    }
}
