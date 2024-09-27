using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Comments : BaseModel
{
    public string Message { get; set; }
    public long UserId { get; set; }
    public long TaskId { get; set; } 
    public virtual Users User { get; set; }
    public virtual Tasks Tasks { get; set; }
}
