using System.Net;
using System.Net.Mail;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Infrastructure
{
    public class SmtpEmailSender : IEmailSender
    {
        private const string _host = "smtp.sib.evraz.com";
        private const string _login = @"SIB\10252.mail";

        //TODO: Сбросить пароль, перенести в переменные
        private const string _pass = "111zzz555HHH&$!";
        private const string _from = "10252.mail@evraz.com";
        private const int _port = 587;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(ILogger<SmtpEmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            SmtpClient emailClient = new()
            {
                Host = _host,
                Port = _port,
                EnableSsl = true,
                Credentials = new NetworkCredential { UserName = _login, Password = _pass }
            };

            MailMessage message = new()
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body,
                From = new MailAddress(_from, "МИП РУК")
            };
            message.To.Add(to);

            await emailClient.SendMailAsync(message);

            _logger.LogWarning("Sending email to {to} from МИП РУК with subject {subject}.", to, subject);
        }
    }
}