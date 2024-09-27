using TaskManager.Models.Base;

namespace TaskManager.Models;

public class UserTasks : BaseModel
{
    public long TaskId { get; set; }

    public long UserId { get; set; }
    public virtual Tasks Task { get; set; }
    public virtual Users User { get; set; }
}
