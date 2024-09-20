namespace TaskManager.Models
{
    public class Themes
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();

        public bool IsDeleted { get; set; }
        public long UserId { get; set; }
        public Users Users { get; set; }

        // New property for collaboration
        public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}