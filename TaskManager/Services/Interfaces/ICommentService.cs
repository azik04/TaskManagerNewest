using TaskManager.Models;
using TaskManager.Response;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Services.Interfaces
{
    public interface ICommentService
    {
        public Task<IBaseResponse<ICollection<Comment>>> GetByTask(int taskId);
        public Task<IBaseResponse<Comment>> Remove(int id);
        public Task<IBaseResponse<Comment>> Create(CreateCommentVM comment);
    }
}
