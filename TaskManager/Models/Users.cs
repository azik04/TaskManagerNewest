using TaskManager.Enum;
using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Users : BaseModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }

    public virtual ICollection<Tasks> Tasks { get; set; }
    public virtual ICollection<UserTask> UserTasks { get; set; }
    public virtual ICollection<Comment> Comment { get; set; }
    public virtual ICollection<Themes> Themes { get; set; }
}
