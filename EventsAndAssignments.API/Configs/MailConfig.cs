using EventsAndAssignments.Services.Extensions;
using EventsAndAssignments.Services.Options;

namespace EventsAndAssignments.API.Configs
{
    public static class MailConfig
    {
        public static MailOptions GetDefaultMailParametrs(WebApplicationBuilder builder)
        {
            MailOptions mail = new()
            {
                User = builder.Configuration["MAIL_USER"] ??
                    builder.Configuration["Mail:User"],

                Host = builder.Configuration["MAIL_HOST"] ??
                    builder.Configuration["Mail:Host"],
                Pass = builder.Configuration["MAIL_PASS"]
            };

            string? mailPort = builder.Configuration["MAIL_PORT"];
            string? frontUrl = builder.Configuration["FrontURL"];

            if (mailPort.HasValue())
            {
                mail.Port = Convert.ToInt32(builder.Configuration["MAIL_PORT"]);
            }
            else
            {
                mail.Port = Convert.ToInt32(builder.Configuration["Mail:Port"]);
            }

            return mail;
        }
    }
}