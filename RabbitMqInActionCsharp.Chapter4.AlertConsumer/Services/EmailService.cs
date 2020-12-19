using System.Net;
using System.Net.Mail;

namespace RabbitMqInActionCsharp.Chapter4.AlertConsumer.Services
{
    public static class EmailService
    {
        public static async void SendMail
        (
            string smtpHost,
            int smtpPort,
            bool smtpUseSsl,
            string smtpUserName,
            string smtpPassword,
            string senderMail,
            string recipientMail,
            string subject,
            string message
        )
        {
            var addressFrom = new MailAddress(senderMail);
            var addressTo = new MailAddress(recipientMail);
            var mail = new MailMessage(addressFrom, addressTo)
            {
                Subject = subject,
                Body = message
            };
            var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUserName, smtpPassword),
                EnableSsl = smtpUseSsl
            };
            await smtp.SendMailAsync(mail);
        }
    }
}