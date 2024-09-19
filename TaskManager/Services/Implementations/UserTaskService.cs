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
        public async Task<bool> AddUserToTask(long taskId, int userId)
        {
            var task = await _db.Tasks.FindAsync(taskId);
            var user = await _db.Users.FindAsync(userId);

            if (task == null || user == null)
                return false;

            var userTask = new UserTask
            {
                TaskId = taskId,
                UserId = userId
            };

            await _db.UserTasks.AddAsync(userTask);
            await _db.SaveChangesAsync();
            //await _mailService.Send("your-email@mail.com", user.Email, "You have been added to a task");
            return true;
        }

        public async Task<bool> RemoveUserFromTask(long taskId, int userId)
        {
            var userTask = await _db.UserTasks
                .FirstOrDefaultAsync(ut => ut.TaskId == taskId && ut.UserId == userId);

            if (userTask == null)
                return false;

            _db.UserTasks.Remove(userTask);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<ICollection<Users>> GetUsersByTaskId(long taskId)
        {
            var users = await _db.UserTasks
                .Where(ut => ut.TaskId == taskId)
                .Select(ut => ut.User)
                .ToListAsync();

            return users;
        }
    }
}
