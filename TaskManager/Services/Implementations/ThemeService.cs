using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Themes;

namespace TaskManager.Services.Implementations;

public class ThemeService : IThemeService
{
    private readonly ApplicationDbContext _db;

    public ThemeService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IBaseResponse<GetThemeVM>> Create(CreateThemeVM theme)
    {
        try
        {
            var data = new Themes()
            {
                Name = theme.Name,
                CreateAt = DateTime.Now,
                UserId = theme.UserId,
            };

            await _db.Themes.AddAsync(data);
            await _db.SaveChangesAsync();

            var vm = new GetThemeVM()
            {
                id = data.Id, 
                Name = theme.Name,
                UserId = theme.UserId,
            };
            Log.Information("Theme {ThemeId} created successfully", data.Id);

            return new BaseResponse<GetThemeVM>()
            {
                Data = vm,
                Description = $"Theme: {data.Id} has been successfully created",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (DbUpdateException dbEx)
        {
            Log.Error(dbEx, "Database update error: {Message}, Inner Exception: {InnerException}", dbEx.Message, dbEx.InnerException?.Message);
            return new BaseResponse<GetThemeVM>()
            {
                Description = dbEx.InnerException?.Message ?? dbEx.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetThemeVM>>> GetAll()
    {
        try
        {
            var data = await _db.Themes.ToListAsync();
            var themeVMs = data.Select(item => new GetThemeVM
            {
                id = item.Id,
                Name = item.Name,
                UserId = item.UserId,
            }).ToList();

            Log.Information("Retrieved all themes successfully");

            return new BaseResponse<ICollection<GetThemeVM>>()
            {
                Data = themeVMs,
                Description = "Themes successfully retrieved",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving themes: {Message}", ex.Message);
            return new BaseResponse<ICollection<GetThemeVM>>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetThemeVM>> GetById(long id)
    {
        try
        {
            var theme = await _db.Themes.SingleOrDefaultAsync(x => x.Id == id);
            if (theme == null)
            {
                Log.Warning("Theme with Id {ThemeId} was not found", id);
                return new BaseResponse<GetThemeVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Theme with Id {id} was not found."
                };
            }

            var vm = new GetThemeVM()
            {
                id = theme.Id,
                Name = theme.Name,
                UserId = theme.UserId,
            };

            Log.Information("Theme with Id {ThemeId} retrieved successfully", theme.Id);

            return new BaseResponse<GetThemeVM>
            {
                Data = vm,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Theme with Id {theme.Id} has been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving theme with Id {ThemeId}: {Message}", id, ex.Message);
            return new BaseResponse<GetThemeVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while retrieving the theme: {ex.Message}"
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetThemeVM>>> GetByUser(long userId)
    {
        try
        {
            var data = await _db.Themes.Where(x => x.UserId == userId && !x.IsDeleted).ToListAsync();
            var themeVMs = data.Select(item => new GetThemeVM
            {
                id = item.Id,
                Name = item.Name,
                UserId = item.UserId,
            }).ToList();

            Log.Information("Retrieved all themes for UserId {UserId} successfully", userId);

            return new BaseResponse<ICollection<GetThemeVM>>()
            {
                Data = themeVMs,
                Description = "Themes successfully retrieved",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving themes for UserId {UserId}: {Message}", userId, ex.Message);
            return new BaseResponse<ICollection<GetThemeVM>>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetThemeVM>> Remove(long id)
    {
        try
        {
            var theme = await _db.Themes.SingleOrDefaultAsync(x => x.Id == id);
            if (theme == null)
            {
                Log.Warning("Theme with Id {ThemeId} not found for removal", id);
                return new BaseResponse<GetThemeVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Theme with Id {id} not found."
                };
            }

            theme.IsDeleted = true;
            theme.DeletedAt = DateTime.Now;

            var tasks = await _db.Tasks.Where(x => x.ThemeId == id).ToListAsync();
            foreach (var task in tasks)
            {
                task.IsDeleted = true;
                task.DeletedAt = DateTime.Now;

                var files = await _db.Files.Where(x => x.TaskId == task.Id).ToListAsync();
                foreach (var file in files)
                {
                    file.IsDeleted = true;
                    file.DeletedAt = DateTime.Now;
                }
                _db.Files.UpdateRange(files); 

                var comments = await _db.Comments.Where(x => x.TaskId == task.Id).ToListAsync();
                foreach (var comment in comments)
                {
                    comment.IsDeleted = true;
                    comment.DeletedAt = DateTime.Now;
                }
                _db.Comments.UpdateRange(comments); 

                var userTasks = await _db.UserTasks.Where(x => x.TaskId == task.Id).ToListAsync();
                foreach (var userTask in userTasks)
                {
                    userTask.IsDeleted = true;
                    userTask.DeletedAt = DateTime.Now;
                }
                _db.UserTasks.UpdateRange(userTasks); 

                var subtasks = await _db.SubTasks.Where(x => x.TaskId == task.Id).ToListAsync();
                foreach (var subtask in subtasks)
                {
                    subtask.IsDeleted = true;
                    subtask.DeletedAt = DateTime.Now;
                }
                _db.SubTasks.UpdateRange(subtasks); 
            }

            _db.Tasks.UpdateRange(tasks); 
            _db.Themes.Update(theme); 
            await _db.SaveChangesAsync();

            var vm = new GetThemeVM
            {
                id = theme.Id,
                Name = theme.Name,
                UserId = theme.UserId,
            };

            Log.Information("Theme with Id {ThemeId} has been successfully removed", theme.Id);
            return new BaseResponse<GetThemeVM>
            {
                Data = vm,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Theme with Id {theme.Id} has been successfully removed."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while removing theme with Id {ThemeId}: {Message}", id, ex.Message);
            return new BaseResponse<GetThemeVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while removing the theme: {ex.Message}"
            };
        }
    }
}
