namespace TaskManager.Models;

public class Files
{
    public long Id { get; set; }
    public string FileName { get; set; } 

    // Foreign key for Task
    public long TaskId { get; set; }
    public Tasks Task { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreateAt { get; set; }
}
