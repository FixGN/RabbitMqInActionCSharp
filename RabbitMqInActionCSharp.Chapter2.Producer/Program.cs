using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using BasicProperties = RabbitMQ.Client.Framing.BasicProperties;

namespace RabbitMqInActionCSharp.Chapter2.Publisher
{
    class Program
    { 
        private const string ExchangeName = "hello-exchange";
        
        static void Main(string[] args)
        {
            // default host is localhost
            var factory = new ConnectionFactory();

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.ExchangeDeclare
            (
                ExchangeName,
                "direct",
                true,
                false,
                new Dictionary<string, object> {{"passive", false}}
            );
            
            var messageProperties = new BasicProperties
            {
                ContentType = "text/plain"
            };
            var messageBody = Encoding.UTF8.GetBytes(args[0]);
            
            channel.BasicPublish
            (
                ExchangeName,
                "hola",
                messageProperties,
                messageBody
            );
        }
    }
}