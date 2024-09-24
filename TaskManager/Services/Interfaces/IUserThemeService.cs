using TaskManager.Models;

namespace TaskManager.Services.Interfaces;

public interface IUserThemeService
{
    Task<bool> AddUsersToTask(long taskId, long userId);
    Task<bool> RemoveUserFromTheme(long taskId, long userId);
    Task<ICollection<Users>> GetUsersByThemeId(long taskId);
    Task<ICollection<Tasks>> GetThemesByUserId(long userId);
}
