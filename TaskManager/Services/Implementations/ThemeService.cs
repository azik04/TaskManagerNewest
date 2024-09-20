using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Themes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManager.Services.Implementations
{
    public class ThemeService : IThemeService
    {
        private readonly ApplicationDbContext _db;
        public ThemeService(ApplicationDbContext db) { _db = db;}
        public async Task<IBaseResponse<Themes>> Create(ThemeVM task)
        {
            try
            {
                var data = new Themes()
                {
                    Name = task.Name,
                    UserId = task.UserId,
                };
                await _db.Themes.AddAsync(data);
                await _db.SaveChangesAsync();
                return new BaseResponse<Themes>()
                {
                    Data = data,
                    Description = $"Theme: {data.Id} has been successfuly Create",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch(Exception ex)
            {
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
                    UserId=item.UserId,
                }).ToList();

                return new BaseResponse<ICollection<ThemeVM>>()
                {
                    Data = themeVMs,
                    Description = "Themes successfully retrieved",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
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
                var task = await _db.Themes
                    .SingleOrDefaultAsync(x => x.Id == id);
                if (task == null)
                {
                    return new BaseResponse<ThemeVM>
                    {
                        StatusCode = Enum.StatusCode.NotFound,
                        Description = $"Task with Id {id} was not found."
                    };
                }
                var vm = new ThemeVM()
                {
                    Name = task.Name,
                    id = task.Id,
                    UserId = task.UserId,
                };
                return new BaseResponse<ThemeVM>
                {
                    Data = vm,
                    StatusCode = Enum.StatusCode.OK,
                    Description = $"Task with Id {task.Id} has been successfully retrieved."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ThemeVM>
                {
                    StatusCode = Enum.StatusCode.Error,
                    Description = $"An error occurred while retrieving the task: {ex.Message}"
                };
            }
        }

        public async Task<IBaseResponse<ICollection<ThemeVM>>> GetByUser(long id)
        {
            try
            {
                var data = await _db.Themes.Where(x => x.UserId ==id).ToListAsync();
                var themeVMs = data.Select(item => new ThemeVM
                {
                    Name = item.Name,
                    id = item.Id,
                    UserId = item.UserId,
                }).ToList();

                return new BaseResponse<ICollection<ThemeVM>>()
                {
                    Data = themeVMs,
                    Description = "Themes successfully retrieved",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
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
                var task = await _db.Themes.SingleOrDefaultAsync(x => x.Id == id);
                if (task == null)
                {
                    return new BaseResponse<Themes>
                    {
                        StatusCode = Enum.StatusCode.NotFound,
                        Description = $"Task with Id {id} not found."
                    };
                }

                var files = await _db.Files.Where(x => x.TaskId == id).ToListAsync();
                foreach (var file in files)
                {
                    file.IsDeleted = true;
                }

                task.IsDeleted = true;
                _db.Themes.Remove(task);
                await _db.SaveChangesAsync();

                return new BaseResponse<Themes>
                {
                    Data = task,
                    StatusCode = Enum.StatusCode.OK,
                    Description = $"Task with Id {task.Id} has been successfully removed."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Themes>
                {
                    StatusCode = Enum.StatusCode.Error,
                    Description = $"An error occurred while removing the task: {ex.Message}"
                };
            }
        }
    }
}
