using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqInActionCsharp.Chapter3.LogListener
{
    class Program
    {
        private const string ExchangeName = "amq.rabbitmq.log";
        
        static void Main()
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var errorsQueue = channel.QueueDeclare();
            var warningsQueue = channel.QueueDeclare();
            var infoQueue = channel.QueueDeclare();
            
            channel.QueueBind(errorsQueue.QueueName, ExchangeName, "error");
            channel.QueueBind(warningsQueue.QueueName, ExchangeName, "warning");
            channel.QueueBind(infoQueue.QueueName, ExchangeName, "info");
            
            var errorConsumer = new EventingBasicConsumer(channel);
            var warningConsumer = new EventingBasicConsumer(channel);
            var infoConsumer = new EventingBasicConsumer(channel);

            errorConsumer.Received += (model, eventArgs) =>
            {
                Console.WriteLine($"[Error] {Encoding.UTF8.GetString(eventArgs.Body)}");
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            
            warningConsumer.Received += (model, eventArgs) =>
            {
                Console.WriteLine($"[Warning] {Encoding.UTF8.GetString(eventArgs.Body)}");
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            
            infoConsumer.Received += (model, eventArgs) =>
            {
                Console.WriteLine($"[Info] {Encoding.UTF8.GetString(eventArgs.Body)}");
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            channel.BasicConsume(errorConsumer, errorsQueue.QueueName);
            channel.BasicConsume(warningConsumer, warningsQueue.QueueName);
            channel.BasicConsume(infoConsumer, infoQueue.QueueName);

            while (true)
            {
                Thread.Sleep(500);
            }
        }
    }
}