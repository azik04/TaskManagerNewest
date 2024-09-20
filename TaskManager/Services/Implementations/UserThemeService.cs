using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations
{
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
                var task = await _db.Themes.FindAsync(themeId);
                var user = await _db.Users.FindAsync(userId);

                if (task == null || user == null)
                    return false;

                // Check if the user-task relationship already exists
                var existingUserTask = await _db.UserTasks
                    .AnyAsync(ut => ut.ThemeId == themeId && ut.UserId == userId);

                if (existingUserTask)
                {
                    Console.WriteLine("This user is already added to the task.");
                    return false; // or handle it as you see fit
                }

                var userTask = new UserTask
                {
                    ThemeId = themeId,
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


        public async Task<bool> RemoveUserFromTheme(long themeId, long userId)
        {
            var userTask = await _db.UserTasks
                .FirstOrDefaultAsync(ut => ut.ThemeId == themeId && ut.UserId == userId);

            if (userTask == null)
                return false;

            _db.UserTasks.Remove(userTask);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<ICollection<Users>> GetUsersByThemeId(long themeId)
        {
            var users = await _db.UserTasks
                .Where(ut => ut.ThemeId == themeId)
                .Select(ut => ut.User)
                .ToListAsync();

            return users;
        }
        public async Task<ICollection<Themes>> GetThemesByUserId(long userId)
       {
            var themes = await _db.UserTasks
                .Where(ut => ut.UserId == userId) 
                .Include(ut => ut.Theme) 
                .Select(ut => ut.Theme) 
                .Distinct() 
                .ToListAsync();

            return themes;
        }

    }
}
