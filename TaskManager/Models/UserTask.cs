namespace TaskManager.Models
{
    public class UserTask
    {
        public long ThemeId { get; set; }
        public Themes Theme { get; set; }

        public long UserId { get; set; }
        public Users User { get; set; }
    }
}
