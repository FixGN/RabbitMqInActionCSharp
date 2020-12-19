using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqInActionCsharp.Chapter4.AlertConsumer.Models;
using RabbitMqInActionCsharp.Chapter4.AlertConsumer.Services;

namespace RabbitMqInActionCsharp.Chapter4.AlertConsumer
{
    class Program
    {
        private const string ExchangeTypeTopic = "topic";
        private const string QueueNameCritical = "critical";
        private const string QueueNameRateLimit = "rate_limit";
        private const string RoutingKeyCritical = "critical.*";
        private const string RoutingKeyRateLimit = ".*rate_limit";
        private const string ConsumerTagCritical = "critical";
        private const string ConsumerTagRateLimit = "rate_limit";
        
        private const string AmqpServerHost = "localhost";
        private const string AmqpUser = "alert_user";
        private const string AmqpPassword = "alertme";
        private const string AmqpVhost = "/";
        private const string ExchangeName = "alerts";
        
        static void Main(string[] args)
        {
            var configuration = Setup.GetConfiguration();
            
            var factory = new ConnectionFactory
            {
                HostName = AmqpServerHost,
                UserName = AmqpUser,
                Password = AmqpPassword,
                VirtualHost = AmqpVhost
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare
            (
                ExchangeName,
                ExchangeTypeTopic,
                true
            );

            channel.QueueDeclare(QueueNameCritical, autoDelete: false);
            channel.QueueBind(QueueNameCritical, ExchangeName, RoutingKeyCritical);

            channel.QueueDeclare(QueueNameRateLimit, autoDelete: false);
            channel.QueueBind(QueueNameRateLimit, ExchangeName, RoutingKeyRateLimit);

            var criticalConsumer = new EventingBasicConsumer(channel);
            var rateLimitConsumer = new EventingBasicConsumer(channel);

            criticalConsumer.Received += (model, eventArgs) =>
            {
                var body = Encoding.UTF8.GetString(eventArgs.Body);
                var alert = JsonSerializer.Deserialize<Alert>(body);

                var recipients = configuration.AlertRecipientsCritical;

                foreach (var recipient in recipients)
                {
                    EmailService.SendMail
                    (
                        configuration.MailSmtpHost,
                        configuration.MailSmtpPort,
                        configuration.MailSmtpUseSsl,
                        configuration.MailUserName,
                        configuration.MailPassword, 
                        configuration.MailAddress,
                        recipient,
                        "Critical alert!",
                        alert.Message
                    );
                }
                
                Console.WriteLine($"Sent alert via e-mail! Alert Text: {alert.Message} \n" +
                                  $" Recipients: {recipients}");

                channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            
            rateLimitConsumer.Received += (model, eventArgs) =>
            {
                var body = Encoding.UTF8.GetString(eventArgs.Body);
                var alert = JsonSerializer.Deserialize<Alert>(body);

                var recipients = configuration.AlertRecipientsRateLimit;

                foreach (var recipient in recipients)
                {
                    EmailService.SendMail
                    (
                        configuration.MailSmtpHost,
                        configuration.MailSmtpPort,
                        configuration.MailSmtpUseSsl,
                        configuration.MailUserName,
                        configuration.MailPassword, 
                        configuration.MailAddress,
                        recipient,
                        "Rate limit alert!",
                        alert.Message
                    );
                }
                
                Console.WriteLine($"Sent alert via e-mail! Alert Text: {alert.Message} \n" +
                                  $" Recipients: {recipients}");

                channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            channel.BasicConsume
            (
                criticalConsumer,
                QueueNameCritical,
                consumerTag: ConsumerTagCritical
            );
            channel.BasicConsume
            (
                rateLimitConsumer,
                QueueNameCritical,
                consumerTag: ConsumerTagRateLimit
            );

            Console.WriteLine("Ready to alerts! For exit enter any key...");
            Console.ReadKey();
        }
    }
}