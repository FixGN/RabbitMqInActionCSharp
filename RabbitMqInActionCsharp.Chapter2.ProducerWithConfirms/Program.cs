using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using BasicProperties = RabbitMQ.Client.Framing.BasicProperties;

namespace RabbitMqInActionCsharp.Chapter2.ProducerWithConfirms
{
    class Program
    { 
        private const string ExchangeName = "hello-exchange";
        private static List<ulong> MessageIds = new List<ulong>();
        
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
            
            channel.ConfirmSelect();
            Console.WriteLine("Channel in 'confirm' mode!");

            channel.BasicAcks += (sender, eventArgs) =>
            {
                if (MessageIds.Contains(eventArgs.DeliveryTag))
                {
                    Console.WriteLine("Confirm received!");
                    MessageIds.Remove(eventArgs.DeliveryTag);
                }
            };
            channel.BasicNacks += (sender, eventArgs) =>
            {
                if (MessageIds.Contains(eventArgs.DeliveryTag))
                {
                    Console.WriteLine("Message lost!");
                }
            };
            
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
            
            MessageIds.Add((ulong) MessageIds.Count + 1);
            
            channel.WaitForConfirmsOrDie();
        }
    }
}