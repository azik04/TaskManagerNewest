namespace TaskManager.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; } 
        public Users User { get; set; }
        public int TaskId { get; set; } 
        public Tasks Tasks { get; set; }
    }
}
