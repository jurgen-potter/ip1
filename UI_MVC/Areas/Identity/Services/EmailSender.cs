using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CitizenPanel.UI.MVC.Areas.Identity.Services;

public class EmailSender(IConfiguration config, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        string host = config["Smtp:Host"];
        int port = int.TryParse(config["Smtp:Port"], out var parsedPort) && parsedPort > 0
            ? parsedPort
            : 587;
        string username = config["Smtp:Username"];
        string password = config["Smtp:Password"];
        string from = config["Smtp:From"];

        try
        {
            using var message = new MailMessage();
            message.From = new MailAddress(from);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(host, port);
            smtpClient.Credentials = new NetworkCredential(username, password);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send email to {To}", to);
        }
    }
}