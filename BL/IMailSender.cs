namespace CitizenPanel.BL;

using Microsoft.Extensions.Configuration;

public interface IMailSender
{
    public Task SendMailAsync(string to, string subject, string body);
}