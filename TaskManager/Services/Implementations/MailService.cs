using MimeKit;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations;

public class MailService : IMailService
{
    private readonly IHostEnvironment _env;

    public MailService(IHostEnvironment env)
    {
        _env = env;
    }

    public async Task Send(string from, string to, string subject)
    {
        try
        {
            string path = Path.Combine(_env.ContentRootPath, "wwwroot", "assets", "Templates", "htmlpage.html");

            string emailTemplate;
            using (StreamReader sourceReader = new StreamReader(path))
            {
                emailTemplate = await sourceReader.ReadToEndAsync();
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sender Name", from));
            message.To.Add(new MailboxAddress("Recipient Name", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailTemplate
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync("hacibalaev.azik@mail.ru", "b7aLR8bgc53xUj99CaET");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
