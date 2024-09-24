using Microsoft.EntityFrameworkCore;
using Serilog;
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

    public async Task<IBaseResponse<Themes>> Create(ThemeVM theme)
    {
        try
        {
            var data = new Themes()
            {
                Name = theme.Name,
                UserId = theme.UserId,
            };

            await _db.Themes.AddAsync(data);
            await _db.SaveChangesAsync();

            Log.Information("Theme {ThemeId} created successfully", data.Id);

            return new BaseResponse<Themes>()
            {
                Data = data,
                Description = $"Theme: {data.Id} has been successfully created",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while creating theme: {Message}", ex.Message);
            return new BaseResponse<Themes>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<ThemeVM>>> GetAll()
    {
        try
        {
            var data = await _db.Themes.ToListAsync();
            var themeVMs = data.Select(item => new ThemeVM
            {
                Name = item.Name,
                id = item.Id,
                UserId = item.UserId,
            }).ToList();

            Log.Information("Retrieved all themes successfully");

            return new BaseResponse<ICollection<ThemeVM>>()
            {
                Data = themeVMs,
                Description = "Themes successfully retrieved",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving themes: {Message}", ex.Message);
            return new BaseResponse<ICollection<ThemeVM>>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ThemeVM>> GetById(long id)
    {
        try
        {
            var theme = await _db.Themes.SingleOrDefaultAsync(x => x.Id == id);
            if (theme == null)
            {
                Log.Warning("Theme with Id {ThemeId} was not found", id);
                return new BaseResponse<ThemeVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Theme with Id {id} was not found."
                };
            }

            var vm = new ThemeVM()
            {
                Name = theme.Name,
                id = theme.Id,
                UserId = theme.UserId,
            };

            Log.Information("Theme with Id {ThemeId} retrieved successfully", theme.Id);

            return new BaseResponse<ThemeVM>
            {
                Data = vm,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Theme with Id {theme.Id} has been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving theme with Id {ThemeId}: {Message}", id, ex.Message);
            return new BaseResponse<ThemeVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while retrieving the theme: {ex.Message}"
            };
        }
    }

    public async Task<IBaseResponse<ICollection<ThemeVM>>> GetByUser(long userId)
    {
        try
        {
            var data = await _db.Themes.Where(x => x.UserId == userId).ToListAsync();
            var themeVMs = data.Select(item => new ThemeVM
            {
                Name = item.Name,
                id = item.Id,
                UserId = item.UserId,
            }).ToList();

            Log.Information("Retrieved all themes for UserId {UserId} successfully", userId);

            return new BaseResponse<ICollection<ThemeVM>>()
            {
                Data = themeVMs,
                Description = "Themes successfully retrieved",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving themes for UserId {UserId}: {Message}", userId, ex.Message);
            return new BaseResponse<ICollection<ThemeVM>>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<Themes>> Remove(long id)
    {
        try
        {
            var theme = await _db.Themes.SingleOrDefaultAsync(x => x.Id == id);
            if (theme == null)
            {
                Log.Warning("Theme with Id {ThemeId} not found for removal", id);
                return new BaseResponse<Themes>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Theme with Id {id} not found."
                };
            }

            var files = await _db.Files.Where(x => x.TaskId == id).ToListAsync();
            foreach (var file in files)
            {
                file.IsDeleted = true;
            }

            theme.IsDeleted = true;
            _db.Themes.Remove(theme);
            await _db.SaveChangesAsync();

            Log.Information("Theme with Id {ThemeId} has been successfully removed", theme.Id);

            return new BaseResponse<Themes>
            {
                Data = theme,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Theme with Id {theme.Id} has been successfully removed."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while removing theme with Id {ThemeId}: {Message}", id, ex.Message);
            return new BaseResponse<Themes>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while removing the theme: {ex.Message}"
            };
        }
    }
}
