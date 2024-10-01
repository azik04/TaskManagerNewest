using TaskManager.Models.Base;

namespace TaskManager.Models;

public class SubTasks: BaseModel
{
    public string Name { get; set; }
    public string Priority { get; set; }
    public long UserId { get; set; }
    public long TaskId { get; set; }
    public bool IsCompleted { get; set; }

    public Users User { get; set; }
    public Tasks Task { get; set; }
}
