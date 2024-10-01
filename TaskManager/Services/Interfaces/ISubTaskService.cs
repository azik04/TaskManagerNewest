using TaskManager.Response;
using TaskManager.ViewModels.SubTask;

namespace TaskManager.Services.Interfaces
{
    public interface ISubTaskService
    {
        public Task<IBaseResponse<ICollection<GetSubTaskVM>>> GetByTask(long taskId);
        public Task<IBaseResponse<ICollection<GetSubTaskVM>>> GetByTaskDone(long taskId);
        public Task<IBaseResponse<ICollection<GetSubTaskVM>>> GetByTaskNotDone(long taskId);
        public Task<IBaseResponse<GetSubTaskVM>> Complete(long id);
        public Task<IBaseResponse<GetSubTaskVM>> Remove(long id);
        public Task<IBaseResponse<GetSubTaskVM>> Create(CreateSubTaskVM subTask);
    }
}
