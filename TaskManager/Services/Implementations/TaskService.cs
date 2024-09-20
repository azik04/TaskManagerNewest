using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Files;
using TaskManager.ViewModels.Tasks;

namespace TaskManager.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _db;
    public TaskService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<IBaseResponse<GetTaskVM>> Create(CreateTaskVM task)
    {
        try
        {
            var data = new Tasks
            {
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                CreateDate = DateTime.Now,
                Status = task.Status,
                DeadLine = task.DeadLine,
                Priority = task.Priority,
                ThemeId = task.ThemeId
            };

            await _db.Tasks.AddAsync(data);
            await _db.SaveChangesAsync();

            // Создание DTO для возвращаемого значения
            var taskDTO = new GetTaskVM
            {
                Id = data.Id,
                TaskName = data.TaskName,
                TaskDescription = data.TaskDescription,
                Status = data.Status,
                Priority = data.Priority,
                DeadLine = data.DeadLine,
                ThemeId = data.ThemeId,
                Files = data.Files.Select(f => new FilesVM
                {
                    Id = f.Id,
                    File = f.FileName
                }).ToList(),
            };

            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task: {taskDTO.Id} has been successfully created."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GetTaskVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while creating the task: {ex.Message}",
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetTaskVM>>> GetAll()
    {
        try
        {
            var tasks = await _db.Tasks
                .Where(x => !x.IsDeleted)
                .Include(x => x.Files)
                .ToListAsync();

            var taskViewModels = new List<GetTaskVM>();

            foreach (var item in tasks)
            {
                var vm = new GetTaskVM
                {
                    Id = item.Id,
                    TaskName = item.TaskName,
                    TaskDescription = item.TaskDescription,
                    Status = item.Status,
                    Priority = item.Priority,
                    DeadLine = item.DeadLine,
                    DateOfCompletion = item.DateOfCompletion,
                    CreateDate = item.CreateDate,
                    IsCompleted = item.IsCompleted,
                    ThemeId = item.ThemeId,
                    Files = item.Files.Select(f => new FilesVM
                    {
                        Id = f.Id,
                        File = f.FileName,
                        IsDeleted = f.IsDeleted
                    }).ToList()
                };

                taskViewModels.Add(vm);
            }

            return new BaseResponse<ICollection<GetTaskVM>>
            {
                Data = taskViewModels,
                StatusCode = Enum.StatusCode.OK,
                Description = "All tasks have been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<ICollection<GetTaskVM>>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while retrieving tasks: {ex.Message}"
            };
        }
    }
    public async Task<IBaseResponse<GetTaskVM>> GetById(long id)
    {
        try
        {
            var task = await _db.Tasks
                .Include(x => x.Files)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return new BaseResponse<GetTaskVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Task with Id {id} was not found."
                };
            }
            
            var taskDTO = new GetTaskVM
            {
                Id = task.Id,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                Status = task.Status,
                Priority = task.Priority,
                DeadLine = task.DeadLine,
                DateOfCompletion = task.DateOfCompletion,
                CreateDate = task.CreateDate,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                Files = task.Files.Select(f => new FilesVM
                {
                    Id = f.Id,
                    File = f.FileName,
                    IsDeleted = f.IsDeleted
                }).ToList()
            };

            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GetTaskVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while retrieving the task: {ex.Message}"
            };
        }
    }

    public async Task<IBaseResponse<GetTaskVM>> Remove(long id)
    {
        try
        {
            var task = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return new BaseResponse<GetTaskVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Task with Id {id} not found."
                };
            }
            task.IsDeleted = true;
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            var taskDTO = new GetTaskVM
            {
                Id = task.Id,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                Status = task.Status,
                Priority = task.Priority,
                DeadLine = task.DeadLine,
                DateOfCompletion = task.DateOfCompletion,
                CreateDate = task.CreateDate,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                Files = task.Files.Select(f => new FilesVM
                {
                    Id = f.Id,
                    File = f.FileName,
                    IsDeleted = f.IsDeleted
                }).ToList()
            };
         

            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully removed."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GetTaskVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while removing the task: {ex.Message}"
            };
        }
    }
    
    public async Task<IBaseResponse<GetTaskVM>> Update(long id, UpdateTaskVM updateTask)
    {
        try
        {
            var task = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return new BaseResponse<GetTaskVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Task with Id {id} not found."
                };
            }

            task.TaskDescription = updateTask.TaskDescription;
            task.TaskName = updateTask.TaskName;
            task.Status = updateTask.Status;
            task.DeadLine = updateTask.DeadLine;
            task.Priority = updateTask.Priority;
            task.DeadLine = updateTask.DeadLine;
            task.ThemeId = updateTask.ThemeId;

            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
            var taskDTO = new GetTaskVM
            {
                Id = task.Id,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                Status = task.Status,
                Priority = task.Priority,
                DeadLine = task.DeadLine,
                DateOfCompletion = task.DateOfCompletion,
                CreateDate = task.CreateDate,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                Files = task.Files.Select(f => new FilesVM
                {
                    Id = f.Id,
                    File = f.FileName,
                    IsDeleted = f.IsDeleted
                }).ToList()
            };
            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully updated."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GetTaskVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while updating the task: {ex.Message}"
            };
        }
    }
        public async Task<IBaseResponse<GetTaskVM>> Complite(long id)
        {
        try
        {
            var task = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return new BaseResponse<GetTaskVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Task with Id {id} not found."
                };
            }

            task.IsCompleted = true;

            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
            var taskDTO = new GetTaskVM
            {
                Id = task.Id,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                Status = task.Status,
                Priority = task.Priority,
                DeadLine = task.DeadLine,
                DateOfCompletion = task.DateOfCompletion,
                CreateDate = task.CreateDate,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                Files = task.Files.Select(f => new FilesVM
                {
                    Id = f.Id,
                    File = f.FileName,
                    IsDeleted = f.IsDeleted
                }).ToList()
            };

            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully updated."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GetTaskVM>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while updating the task: {ex.Message}"
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetTaskVM>>> GetAllDone(long themeId)
    {
        try
        {
            var tasks = await _db.Tasks
                .Where(x => !x.IsDeleted && x.IsCompleted && x.ThemeId == themeId)
                .Include(x => x.Files)
                .ToListAsync();

            var taskViewModels = new List<GetTaskVM>();

            foreach (var item in tasks)
            {
                var vm = new GetTaskVM
                {
                    Id = item.Id,
                    TaskName = item.TaskName,
                    TaskDescription = item.TaskDescription,
                    Status = item.Status,
                    Priority = item.Priority,
                    DeadLine = item.DeadLine,
                    DateOfCompletion = item.DateOfCompletion,
                    CreateDate = item.CreateDate,
                    IsCompleted = item.IsCompleted,
                    ThemeId = item.ThemeId,
                    Files = item.Files.Select(f => new FilesVM
                    {
                        Id = f.Id,
                        File = f.FileName,
                        IsDeleted = f.IsDeleted
                    }).ToList()
                };

                taskViewModels.Add(vm);
            }

            return new BaseResponse<ICollection<GetTaskVM>>
            {
                Data = taskViewModels,
                StatusCode = Enum.StatusCode.OK,
                Description = "All tasks have been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<ICollection<GetTaskVM>>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while retrieving tasks: {ex.Message}"
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetTaskVM>>> GetAllNotDone(long themeId)
    {
        try
        {
            var tasks = await _db.Tasks
               .Where(x => !x.IsDeleted && !x.IsCompleted && x.ThemeId == themeId)
               .Include(x => x.Files)
                .ToListAsync();

            var taskViewModels = new List<GetTaskVM>();

            foreach (var item in tasks)
            {
                var vm = new GetTaskVM
                {
                    Id = item.Id,
                    TaskName = item.TaskName,
                    TaskDescription = item.TaskDescription,
                    Status = item.Status,
                    Priority = item.Priority,
                    DeadLine = item.DeadLine,
                    DateOfCompletion = item.DateOfCompletion,
                    CreateDate = item.CreateDate,
                    IsCompleted = item.IsCompleted,
                    ThemeId = item.ThemeId,
                    Files = item.Files.Select(f => new FilesVM
                    {
                        Id = f.Id,
                        File = f.FileName,
                        IsDeleted = f.IsDeleted
                    }).ToList()
                };

                taskViewModels.Add(vm);
            }

            return new BaseResponse<ICollection<GetTaskVM>>
            {
                Data = taskViewModels,
                StatusCode = Enum.StatusCode.OK,
                Description = "All tasks have been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<ICollection<GetTaskVM>>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while retrieving tasks: {ex.Message}"
            };
        }
    }
}
