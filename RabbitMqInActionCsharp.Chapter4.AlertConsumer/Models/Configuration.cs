namespace RabbitMqInActionCsharp.Chapter4.AlertConsumer.Models
{
    public class Configuration
    {
        public string MailSmtpHost { get; set; }
        public int MailSmtpPort { get; set; }
        public bool MailSmtpUseSsl { get; set; }
        public string MailUserName { get; set; }
        public string MailPassword { get; set; }
        public string MailAddress { get; set; }
        public string[] AlertRecipientsCritical { get; set; }
        public string[] AlertRecipientsRateLimit { get; set; }
    }
}