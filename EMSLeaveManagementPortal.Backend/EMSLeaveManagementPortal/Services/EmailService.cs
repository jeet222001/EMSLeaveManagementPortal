using System.Net;
using System.Net.Mail;

namespace EMSLeaveManagementPortal.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);
        var smtpUser = _config["Smtp:From"];
        var smtpPass = _config["Smtp:Password"];
        var from = _config["Smtp:From"];

        var toAddress = new MailAddress(to);
        var fromAddress = new MailAddress(from);

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };
        var mail = new MailMessage(from, to, subject, body);
        await client.SendMailAsync(mail);
    }
}
