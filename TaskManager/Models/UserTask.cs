namespace TaskManager.Models;

public class UserTask
{
    public long TaskId { get; set; }
    public Tasks Task { get; set; }

    public long UserId { get; set; }
    public Users User { get; set; }
}
