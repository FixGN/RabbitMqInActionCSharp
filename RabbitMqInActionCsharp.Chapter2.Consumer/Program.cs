using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqInActionCsharp.Chapter2.Consumer
{
    class Program
    {
        private const string ExchangeName = "hello-exchange";
        private const string QueueName = "hello-queue";
        private const string RoutingKey = "hola";
        private const string ConsumerTag = "hello-consumer";

        private static bool _consume = true;
        
        static void Main()
        {
            var factory = new ConnectionFactory();

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare
            (
                ExchangeName,
                "direct",
                true,
                false
            );

            channel.QueueDeclare(QueueName, exclusive: false);
            channel.QueueBind(QueueName, ExchangeName, RoutingKey);
            
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                channel.BasicAck(eventArgs.DeliveryTag, false);
                
                var body = Encoding.UTF8.GetString(eventArgs.Body);
                
                if (body == "quit")
                {
                    channel.BasicCancel(ConsumerTag);
                    _consume = false;
                }
                else
                {
                    Console.WriteLine(body);
                }
            };
            
            channel.BasicConsume(consumer, QueueName, false, ConsumerTag);
            ConsumingMessage();
        }

        private static void ConsumingMessage()
        {
            while (_consume)
            {
                Thread.Sleep(500);
            }
        }
    }
}