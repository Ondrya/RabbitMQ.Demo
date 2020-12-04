using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Demo.Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            int reader = random.Next(5);

            Console.WriteLine($"Reader {reader} starts working!");


            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "demo", type: "topic", durable: true);
                var queueName = channel.QueueDeclare().QueueName;

                var bindingKey = $"reader_{reader}";

                channel.QueueBind(
                    queue: queueName,
                    exchange: "demo",
                    routingKey: bindingKey);
                
                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine($" [x] Reader_{reader} received '{routingKey}':'{message}'");
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }


            Console.WriteLine($"Reader {reader} finish working!");
        }
    }
}
