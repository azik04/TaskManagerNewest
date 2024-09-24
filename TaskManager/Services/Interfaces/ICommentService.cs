using TaskManager.Models;
using TaskManager.Response;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Services.Interfaces;

public interface ICommentService
{
    public Task<IBaseResponse<ICollection<Comment>>> GetByTask(long taskId);
    public Task<IBaseResponse<Comment>> Remove(long id);
    public Task<IBaseResponse<Comment>> Create(CreateCommentVM comment);
}
