using TaskManager.Enum;

namespace TaskManager.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool isDeleted { get; set; }
        public Role Role { get; set; }

        // New property for collaboration
        public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>(); 
    }
}
