using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Themes : BaseModel
{
    public string Name { get; set; }
    public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();

    public long UserId { get; set; }
    public Users Users { get; set; }


}