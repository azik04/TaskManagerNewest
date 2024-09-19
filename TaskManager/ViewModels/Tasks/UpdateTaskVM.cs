﻿namespace TaskManager.ViewModels.Tasks
{
    public class UpdateTaskVM
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int ThemeId { get; set; }
        public string DeadLine { get; set; }
        public string Participates { get; set; }
    }
}
