using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Files : BaseModel
{
    public string FileName { get; set; } 

    public long TaskId { get; set; }
    public virtual Tasks Task { get; set; }

}
