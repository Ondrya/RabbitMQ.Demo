using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Demo.Sender.Routing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Sending!");

            var random = new Random();
            int reader = 0;

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "demoDirect",
                                        type: "direct",
                                        durable: true,
                                        autoDelete: false);

                var total = 500;
                for (int i = 1; i < total + 1; i++)
                {
                    reader = random.Next(3);
                    var routingKey = $"reader_{reader}";
                    var message = $"message for reader {reader} datetime {DateTime.Now} {i.ToString("D3")} of {total}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "demoDirect",
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");
                    System.Threading.Thread.Sleep(500);
                }
            }

            Console.WriteLine("End Sending!");
        }
    }
}
