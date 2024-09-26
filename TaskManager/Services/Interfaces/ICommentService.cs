using TaskManager.Response;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Services.Interfaces;

public interface ICommentService
{
    public Task<IBaseResponse<ICollection<GetCommentVM>>> GetByTask(long taskId);
    public Task<IBaseResponse<GetCommentVM>> Remove(long id);
    public Task<IBaseResponse<GetCommentVM>> Create(CreateCommentVM comment);
}
