using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Xml.Linq;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
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
                CreateAt = DateTime.Now,
                Status = task.Status,
                DeadLine = task.DeadLine,
                Priority = task.Priority,
                ThemeId = task.ThemeId,
                ExecutiveUserId = task.ExecutiveUserId,
            };

            await _db.Tasks.AddAsync(data);
            await _db.SaveChangesAsync();

            var taskDTO = new GetTaskVM
            {
                Id = data.Id,
                TaskName = data.TaskName,
                TaskDescription = data.TaskDescription,
                Status = data.Status,
                Priority = data.Priority,
                DeadLine = data.DeadLine,
                ThemeId = data.ThemeId,
                ExecutiveUserId = data.ExecutiveUserId,

            };
            Log.Information("Task {TaskId} created successfully", taskDTO.Id);
            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task: {taskDTO.Id} has been successfully created."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while creating task: {Message}", ex.Message);
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

            var taskViewModels = tasks.Select(item => new GetTaskVM
            {
                Id = item.Id,
                TaskName = item.TaskName,
                TaskDescription = item.TaskDescription,
                Status = item.Status,
                Priority = item.Priority,
                DeadLine = item.DeadLine,
                DateOfCompletion = item.DateOfCompletion,
                CreateDate = item.CreateAt,
                IsCompleted = item.IsCompleted,
                ThemeId = item.ThemeId,
                ExecutiveUserId = item.ExecutiveUserId,

            }).ToList();

            Log.Information("Retrieved all tasks successfully");
            return new BaseResponse<ICollection<GetTaskVM>>
            {
                Data = taskViewModels,
                StatusCode = Enum.StatusCode.OK,
                Description = "All tasks have been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving tasks: {Message}", ex.Message);
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
                Log.Warning("Task with Id {TaskId} was not found", id);
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
                CreateDate = task.CreateAt,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                ExecutiveUserId = task.ExecutiveUserId,
            };

            Log.Information("Task with Id {TaskId} retrieved successfully", task.Id);
            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving task with Id {TaskId}: {Message}", id, ex.Message);
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
                Log.Warning("Task with Id {TaskId} not found for removal", id);
                return new BaseResponse<GetTaskVM>
                {
                    StatusCode = Enum.StatusCode.NotFound,
                    Description = $"Task with Id {id} not found."
                };
            }

            task.IsDeleted = true;
            task.DeletedAt = DateTime.Now;

            var files = await _db.Files.Where(x => x.TaskId == id).ToListAsync();
            foreach (var file in files)
            {
                file.IsDeleted = true;
                file.DeletedAt = DateTime.Now; 
            }

            var comments = await _db.Comments.Where(x => x.TaskId == id).ToListAsync();
            foreach (var comment in comments)
            {
                comment.IsDeleted = true;
                comment.DeletedAt = DateTime.Now; 
            }
            var added = await _db.UserTasks.Where(x => x.TaskId == id).ToListAsync();
            foreach (var item in added)
            {
                item.IsDeleted = true;
                item.DeletedAt = DateTime.Now;
            }
            var sub = await _db.SubTasks.Where(x => x.TaskId == id).ToListAsync();
            foreach (var item in sub)
            {
                item.IsDeleted = true;
                item.DeletedAt = DateTime.Now;
            }
            _db.Tasks.Update(task);
            _db.SubTasks.UpdateRange(sub);
            _db.Files.UpdateRange(files);
            _db.UserTasks.UpdateRange(added);
            _db.Comments.UpdateRange(comments);

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
                CreateDate = task.CreateAt,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                ExecutiveUserId = task.ExecutiveUserId,
            };

            Log.Information("Task with Id {TaskId} has been successfully removed", task.Id);
            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully removed."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while removing task with Id {TaskId}: {Message}", id, ex.Message);
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
                Log.Warning("Task with Id {TaskId} not found for update", id);
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
                CreateDate = task.CreateAt,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                ExecutiveUserId = task.ExecutiveUserId,

            };

            Log.Information("Task with Id {TaskId} has been successfully updated", task.Id);
            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully updated."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while updating task with Id {TaskId}: {Message}", id, ex.Message);
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
                Log.Warning("Task with Id {TaskId} not found for completion", id);
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
                CreateDate = task.CreateAt,
                IsCompleted = task.IsCompleted,
                ThemeId = task.ThemeId,
                ExecutiveUserId = task.ExecutiveUserId,

            };

            Log.Information("Task with Id {TaskId} has been marked as completed", task.Id);
            return new BaseResponse<GetTaskVM>
            {
                Data = taskDTO,
                StatusCode = Enum.StatusCode.OK,
                Description = $"Task with Id {task.Id} has been successfully updated."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while completing task with Id {TaskId}: {Message}", id, ex.Message);
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
            .Include(x => x.ExecutiveUser)
            .ToListAsync();

        var taskViewModels = tasks.Select(item => new GetTaskVM
        {
            Id = item.Id,
            TaskName = item.TaskName,
            TaskDescription = item.TaskDescription,
            Status = item.Status,
            Priority = item.Priority,
            DeadLine = item.DeadLine,
            DateOfCompletion = item.DateOfCompletion,
            CreateDate = item.CreateAt,
            IsCompleted = item.IsCompleted,
            IsDeleted = item.IsDeleted, 
            ThemeId = item.ThemeId,
            ExecutiveUserId = item.ExecutiveUserId,
        }).ToList();

        Log.Information("Retrieved all completed tasks for theme Id {ThemeId} successfully", themeId);
        return new BaseResponse<ICollection<GetTaskVM>>
        {
            Data = taskViewModels,
            StatusCode = Enum.StatusCode.OK,
            Description = "All completed tasks have been successfully retrieved."
        };
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error occurred while retrieving completed tasks for theme Id {ThemeId}: {Message}", themeId, ex.Message);
        return new BaseResponse<ICollection<GetTaskVM>>
        {
            StatusCode = Enum.StatusCode.Error,
            Description = $"An error occurred while retrieving completed tasks: {ex.Message}"
        };
    }
}



    public async Task<IBaseResponse<ICollection<GetTaskVM>>> GetAllNotDone(long themeId)
    {
        try
        {
            var tasks = await _db.Tasks
                .Where(x => !x.IsDeleted && !x.IsCompleted && x.ThemeId == themeId)
                .ToListAsync();

            var taskViewModels = tasks.Select(item => new GetTaskVM
            {
                Id = item.Id,
                TaskName = item.TaskName,
                TaskDescription = item.TaskDescription,
                Status = item.Status,
                Priority = item.Priority,
                DeadLine = item.DeadLine,
                DateOfCompletion = item.DateOfCompletion,
                CreateDate = item.CreateAt,
                IsCompleted = item.IsCompleted,
                ThemeId = item.ThemeId,
                ExecutiveUserId = item.ExecutiveUserId,
            }).ToList();

            Log.Information("Retrieved all not completed tasks for theme Id {ThemeId} successfully", themeId);
            return new BaseResponse<ICollection<GetTaskVM>>
            {
                Data = taskViewModels,
                StatusCode = Enum.StatusCode.OK,
                Description = "All not completed tasks have been successfully retrieved."
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving not completed tasks for theme Id {ThemeId}: {Message}", themeId, ex.Message);
            return new BaseResponse<ICollection<GetTaskVM>>
            {
                StatusCode = Enum.StatusCode.Error,
                Description = $"An error occurred while retrieving not completed tasks: {ex.Message}"
            };
        }
    }
}
