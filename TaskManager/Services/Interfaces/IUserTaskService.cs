using TaskManager.Models;

namespace TaskManager.Services.Interfaces;

public interface IUserTaskService
{
    Task<bool> AddUsersToTask(long taskId, long userId);
    Task<bool> RemoveUserFromTask(long taskId, long userId);
    Task<ICollection<Users>> GetUsersByTaskId(long taskId);
    Task<ICollection<Tasks>> GetTaskByUserId(long userId);
}
