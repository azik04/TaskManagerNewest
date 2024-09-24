namespace TaskManager.ViewModels.Tasks;

public class CreateTaskVM
{
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public long ThemeId { get; set; }
    public string DeadLine { get; set; }
    public long ExecutiveUserId { get; set; }


}
