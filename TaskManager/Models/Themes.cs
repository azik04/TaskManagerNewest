using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Themes : BaseModel
{
    public string Name { get; set; }

    public long UserId { get; set; }
    public virtual Users Users { get; set; }
    public virtual ICollection<Tasks> Tasks { get; set; }

}