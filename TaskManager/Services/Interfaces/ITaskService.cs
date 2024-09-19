using TaskManager.Models;
using TaskManager.Response;
using TaskManager.ViewModels;
using TaskManager.ViewModels.Tasks;

namespace TaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        public Task<IBaseResponse<GetTaskVM>> Create(CreateTaskVM task);
        public Task<IBaseResponse<ICollection<GetTaskVM>>> GetAll();
        public Task<IBaseResponse<ICollection<GetTaskVM>>> GetAllDone(int themeId);
        public Task<IBaseResponse<ICollection<GetTaskVM>>> GetAllNotDone(int themeId);
        public Task<IBaseResponse<GetTaskVM>> GetById(int id);
        public Task<IBaseResponse<GetTaskVM>> Remove(int id);
        public Task<IBaseResponse<GetTaskVM>> Update(int id, UpdateTaskVM task);
        public Task<IBaseResponse<GetTaskVM>> Complite(int id);

    }
}
