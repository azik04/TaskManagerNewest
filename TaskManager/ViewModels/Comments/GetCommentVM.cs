using TaskManager.Models;
using TaskManager.ViewModels.Tasks;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.ViewModels.Comments
{
    public class GetCommentVM
    {
        public string Message { get; set; }
        public long UserId { get; set; }
        public UserVM User { get; set; }
        public long TaskId { get; set; }
        public GetTaskVM Tasks { get; set; }
    }
}
