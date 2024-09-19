namespace TaskManager.Models
{
    public class Themes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();

        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
        public Users Users { get; set; }
    }
}