namespace TaskManager.Services.Interfaces;

public interface IMailService
{
    Task Send(string from, string to, string subject);
}
