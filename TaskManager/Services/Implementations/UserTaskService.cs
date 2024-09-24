using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations
{
    public class UserTaskService : IUserTaskService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMailService _mailService;

        public UserTaskService(ApplicationDbContext db, IMailService mailService)
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
                    return false;

                // Check if the user-task relationship already exists
                var existingUserTask = await _db.UserTasks
                    .AnyAsync(ut => ut.TaskId == themeId && ut.UserId == userId);

                if (existingUserTask)
                {
                    Console.WriteLine("This user is already added to the task.");
                    return false; // or handle it as you see fit
                }

                var userTask = new UserTask
                {
                    TaskId = themeId,
                    UserId = userId
                };

                await _db.UserTasks.AddAsync(userTask);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (you could use a logging library)
                Console.WriteLine($"Error adding user to task: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> RemoveUserFromTask(long themeId, long userId)
        {
            var userTask = await _db.UserTasks
                .FirstOrDefaultAsync(ut => ut.TaskId == themeId && ut.UserId == userId);

            if (userTask == null)
                return false;

            _db.UserTasks.Remove(userTask);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<ICollection<Users>> GetUsersByTaskId(long themeId)
        {
            var users = await _db.UserTasks
                .Where(ut => ut.TaskId == themeId)
                .Select(ut => ut.User)
                .ToListAsync();

            return users;
        }
        public async Task<ICollection<Tasks>> GetTaskByUserId(long userId)
        {
            var themes = await _db.UserTasks
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.Task)
                .Select(ut => ut.Task)
                .Distinct()
                .ToListAsync();

            return themes;
        }
    }
}
