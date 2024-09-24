namespace TaskManager.Models.Base
{
    public class BaseModel
    {
        public long Id { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
