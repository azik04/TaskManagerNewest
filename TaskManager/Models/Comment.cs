using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Comment : BaseModel
{
    public string Message { get; set; }
    public string UserName { get; set; }
    public Users User { get; set; }
    public long TaskId { get; set; } 
    public Tasks Tasks { get; set; }
}
