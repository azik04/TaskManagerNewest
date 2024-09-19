using TaskManager.Models;

namespace TaskManager.Services.Interfaces
{
    public interface IUserTaskService
    {
        Task<bool> AddUserToTask(long taskId, int userId);
        Task<bool> RemoveUserFromTask(long taskId, int userId);
        Task<ICollection<Users>> GetUsersByTaskId(long taskId);
    }
}
