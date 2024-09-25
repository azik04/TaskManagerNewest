namespace TaskManager.ViewModels.Comments;

public class GetCommentVM
{
    public long Id { get; set; }
    public string Message { get; set; }
    public long UserId { get; set; }
    public long TaskId { get; set; }
    public DateTime CreateAt { get; set; }
}
