using EventsAndAssignments.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Infrastructure
{
    public class FakeEmailSender : IEmailSender
    {
        private readonly ILogger<FakeEmailSender> _logger;

        public FakeEmailSender(ILogger<FakeEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation("Not actually sending an email to {to} from МИП with subject {subject}", to, subject);
            return Task.CompletedTask;
        }
    }
}