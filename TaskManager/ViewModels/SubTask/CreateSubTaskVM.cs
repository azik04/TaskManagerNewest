namespace TaskManager.ViewModels.SubTask;

public class CreateSubTaskVM
{
    public string Name { get; set; }
    public string Priority { get; set; }
    public long UserId { get; set; }
    public long TaskId { get; set; }
}
