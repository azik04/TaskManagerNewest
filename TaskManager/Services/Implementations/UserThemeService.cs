using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations;

public class UserThemeService : IUserThemeService
{
    private readonly ApplicationDbContext _db;
    private readonly IMailService _mailService;

    public UserThemeService(ApplicationDbContext db, IMailService mailService)
    {
        _db = db;
        _mailService = mailService;
    }

    public async Task<bool> AddUsersToTask(long themeId, long userId)
    {
        try
        {
            var task = await _db.Tasks.FindAsync(themeId);
            var user = await _db.Users.FindAsync(userId);

            if (task == null || user == null)
            {
                Log.Warning("Task with Id {ThemeId} or User with Id {UserId} not found.", themeId, userId);
                return false;
            }

            var existingUserTask = await _db.UserTasks
                .AnyAsync(ut => ut.TaskId == themeId && ut.UserId == userId);

            if (existingUserTask)
            {
                Log.Information("User with Id {UserId} is already added to the task with Id {ThemeId}.", userId, themeId);
                return false;
            }

            var userTask = new UserTask
            {
                TaskId = themeId,
                UserId = userId
            };

            await _db.UserTasks.AddAsync(userTask);
            await _db.SaveChangesAsync();

            Log.Information("User with Id {UserId} successfully added to task with Id {ThemeId}.", userId, themeId);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding user with Id {UserId} to task with Id {ThemeId}: {Message}", userId, themeId, ex.Message);
            return false;
        }
    }

    public async Task<bool> RemoveUserFromTheme(long themeId, long userId)
    {
        try
        {
            var userTask = await _db.UserTasks
                .FirstOrDefaultAsync(ut => ut.TaskId == themeId && ut.UserId == userId);

            if (userTask == null)
            {
                Log.Warning("No association found for UserId {UserId} and ThemeId {ThemeId}.", userId, themeId);
                return false;
            }

            _db.UserTasks.Remove(userTask);
            await _db.SaveChangesAsync();

            Log.Information("User with Id {UserId} removed from task with Id {ThemeId}.", userId, themeId);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing user with Id {UserId} from task with Id {ThemeId}: {Message}", userId, themeId, ex.Message);
            return false;
        }
    }

    public async Task<ICollection<Users>> GetUsersByThemeId(long themeId)
    {
        try
        {
            var users = await _db.UserTasks
                .Where(ut => ut.TaskId == themeId)
                .Select(ut => ut.User)
                .ToListAsync();

            Log.Information("Retrieved {UserCount} users for task with Id {ThemeId}.", users.Count, themeId);
            return users;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving users for task with Id {ThemeId}: {Message}", themeId, ex.Message);
            return new List<Users>();
        }
    }

    public async Task<ICollection<Tasks>> GetThemesByUserId(long userId)
    {
        try
        {
            var themes = await _db.UserTasks
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.Task)
                .Select(ut => ut.Task)
                .Distinct()
                .ToListAsync();

            Log.Information("Retrieved {ThemeCount} themes for user with Id {UserId}.", themes.Count, userId);
            return themes;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving themes for user with Id {UserId}: {Message}", userId, ex.Message);
            return new List<Tasks>(); 
        }
    }
}
