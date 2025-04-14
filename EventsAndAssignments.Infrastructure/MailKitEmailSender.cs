using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EventsAndAssignments.Infrastructure
{
    public class MailKitEmailSender : IEmailSender
    {
        const string _senderDisplayName = "МИП РУК";
        private const string _from = "10252.mail@evraz.com";
        private readonly ILogger<MailKitEmailSender> _logger;
        private readonly IOptions<MailOptions> _options;

        public MailKitEmailSender(ILogger<MailKitEmailSender> logger, IOptions<MailOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation("Переменная Host: {Host} MAIL__PORT: {Mail} MAIL__USER: {User},",
                _options.Value.Host, _options.Value.Port, _options.Value.User);
            MimeMessage message = new();
            message.From.Add(new MailboxAddress(_senderDisplayName, _from));
            message.To.Add(new MailboxAddress(string.Empty, to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using SmtpClient client = new();
            client.CheckCertificateRevocation = false;
            await client.ConnectAsync(_options.Value.Host, _options.Value.Port);
            await client.AuthenticateAsync(_options.Value.User, _options.Value.Pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogWarning("Sending email to {to} from МИП РУК with subject {subject}. Body: {body}", to, subject, body);
        }
    }
}