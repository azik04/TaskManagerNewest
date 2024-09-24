using TaskManager.Response;
using TaskManager.ViewModels.Tasks;

namespace TaskManager.Services.Interfaces;

public interface ITaskService
{
    public Task<IBaseResponse<GetTaskVM>> Create(CreateTaskVM task);
    public Task<IBaseResponse<ICollection<GetTaskVM>>> GetAll();
    public Task<IBaseResponse<ICollection<GetTaskVM>>> GetAllDone(long themeId);
    public Task<IBaseResponse<ICollection<GetTaskVM>>> GetAllNotDone(long themeId);
    public Task<IBaseResponse<GetTaskVM>> GetById(long id);
    public Task<IBaseResponse<GetTaskVM>> Remove(long id);
    public Task<IBaseResponse<GetTaskVM>> Update(long id, UpdateTaskVM task);
    public Task<IBaseResponse<GetTaskVM>> Complite(long id);

}
