namespace CitizenPanel.BL;

public interface IMailSender
{
    public Task SendMailAsync(string to, string subject, string body);
}