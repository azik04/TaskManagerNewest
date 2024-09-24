using TaskManager.Enum;
using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Users : BaseModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }

    public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>(); 
}
