namespace CitizenPanel.BL.Email;

public interface IEmailSender
{
    public Task SendEmailAsync(string to, string subject, string body);
}