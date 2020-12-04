using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.Demo.Sender
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
                channel.ExchangeDeclare(exchange: "demo",
                                        durable: true,
                                        type: "topic");

                var total = 10000;
                for (int i = 1; i < total+1; i++)
                {
                    reader = random.Next(5);
                    var routingKey = $"reader_{reader}";
                    var message = $"message for reader {reader} datetime {DateTime.UtcNow} {i.ToString("D3")} of {total}";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "demo",
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: body);
                    
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
                    System.Threading.Thread.Sleep(1000);
                }
            }

            Console.WriteLine("End Sending!");
        }
    }
}
