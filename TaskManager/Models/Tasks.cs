﻿namespace TaskManager.Models;

public class Tasks
{
    public long Id { get; set; }
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public string DeadLine { get; set; }
    public long ThemeId { get; set; }
    public DateTime DateOfCompletion { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Files> Files { get; set; } = new List<Files>();
    public Themes Theme { get; set; }

    public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
}