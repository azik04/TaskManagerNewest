using TaskManager.Models.Base;

namespace TaskManager.Models;

public class Tasks : BaseModel
{
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public string DeadLine { get; set; }
    public long ThemeId { get; set; }
    public DateTime DateOfCompletion { get; set; }
    public bool IsCompleted { get; set; }
    public long ExecutiveUserId { get; set; }

    public virtual Users ExecutiveUser { get; set; }
    public virtual Themes Theme { get; set; }
    public virtual ICollection<Files> Files { get; set; } 
    public virtual ICollection<UserTasks> UserTasks { get; set; }
    public virtual ICollection<Comments> Comments { get; set; }
    public virtual ICollection<SubTasks> CoTasks { get; set; }
}