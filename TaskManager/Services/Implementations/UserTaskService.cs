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

                var existingUserTask = await _db.UserTasks
                    .FirstOrDefaultAsync(ut => ut.TaskId == themeId && ut.UserId == userId);

                if (existingUserTask != null)
                {
                    if (existingUserTask.IsDeleted)
                    {
                        existingUserTask.IsDeleted = false;
                        existingUserTask.CreateAt = DateTime.Now; 
                        _db.UserTasks.Update(existingUserTask);
                        await _db.SaveChangesAsync();
                        Console.WriteLine("User task restored.");
                        return true;
                    }

                    Console.WriteLine("This user is already added to the task.");
                    return false;
                }

                var userTask = new UserTask
                {
                    TaskId = themeId,
                    UserId = userId,
                    CreateAt = DateTime.Now,
                    IsDeleted = false 
                };

                await _db.UserTasks.AddAsync(userTask);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
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
            userTask.IsDeleted = true;
            userTask.DeletedAt = DateTime.Now;
            _db.UserTasks.Update(userTask);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<ICollection<Users>> GetUsersByTaskId(long themeId)
        {
            var users = await _db.UserTasks
                .Where(ut => ut.TaskId == themeId && !ut.IsDeleted)
                .Select(ut => ut.User)
                .ToListAsync();

            return users;
        }
        public async Task<ICollection<Tasks>> GetTaskByUserId(long userId)
        {
            var themes = await _db.UserTasks
                .Where(ut => ut.UserId == userId && !ut.IsDeleted)
                .Include(ut => ut.Task)
                .Select(ut => ut.Task)
                .Distinct()
                .ToListAsync();

            return themes;
        }
    }
}
