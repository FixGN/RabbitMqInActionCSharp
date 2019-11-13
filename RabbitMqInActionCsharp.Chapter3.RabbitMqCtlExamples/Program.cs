using System.Collections.Generic;
using RabbitMQ.Client;

namespace RabbitMqCtlExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.ExchangeDeclare
            (
                "logs-exchange",
                "topic",
                true,
                false,
                new Dictionary<string, object> {{"passive", false}}
            );
            
            channel.QueueDeclare("msg-inbox-errors", exclusive: false);
            channel.QueueDeclare("msg-inbox-logs", exclusive: false);
            channel.QueueDeclare("all-logs", exclusive: false);
            
            channel.QueueBind("all-logs", "logs-exchange", "#");
            channel.QueueBind("msg-inbox-errors", "logs-exchange", "error.msg-inbox");
            channel.QueueBind("msg-inbox-logs", "logs-exchange", "*.msg-inbox");
        }
    }
}