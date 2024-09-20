namespace TaskManager.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public Users User { get; set; }
        public string UserName { get; set; }
        public long TaskId { get; set; } 
        public Tasks Tasks { get; set; }
    }
}
