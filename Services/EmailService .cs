using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TaskManagementBackend.Controllers;
using TaskManagementBackend.Models;

public interface IEmailService
{
    Task SendReminderEmail(string email, TaskItem taskItem);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public async Task SendReminderEmail(string email, TaskItem taskItem)
    {
        try
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                client.EnableSsl = _emailSettings.EnableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.From),
                    Subject = $"Reminder: {taskItem.Title}",
                    Body = $"Don't forget your task: {taskItem.Description}. It's due on {taskItem.DueDate}.",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
        catch (SmtpException ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"SMTP Exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Log other exceptions
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

}
