using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.SubTask;

namespace TaskManager.Services.Implementations;

public class SubTaskService : ISubTaskService
{
    private readonly ApplicationDbContext _db;

    public SubTaskService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IBaseResponse<GetSubTaskVM>> Create(CreateSubTaskVM subTask)
    {
        try
        {
            var data = new SubTasks()
            {
                CreateAt = DateTime.Now,
                Name = subTask.Name,
                Priority = subTask.Priority,
                TaskId = subTask.TaskId,
                UserId = subTask.UserId,
            };

            await _db.SubTasks.AddAsync(data);
            await _db.SaveChangesAsync();

            Log.Information("SubTask '{SubTaskName}' created successfully with TaskId: {TaskId}", data.Name, data.TaskId);

            var vm = new GetSubTaskVM()
            {
                Name = subTask.Name,
                Priority = subTask.Priority,
                TaskId = subTask.TaskId,
                UserId = subTask.UserId,
            };

            return new BaseResponse<GetSubTaskVM>
            {
                Data = vm,
                Description = $"SubTask '{subTask.Name}' created successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating SubTask: {SubTaskName}", subTask.Name);

            return new BaseResponse<GetSubTaskVM>
            {
                Description = "An error occurred while creating the SubTask.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetSubTaskVM>>> GetByTask(long taskId)
    {
        try
        {
            var taskExist = await _db.SubTasks.Where(x => x.TaskId == taskId && !x.IsDeleted).ToListAsync();
            if (taskExist == null)
            {
                return new BaseResponse<ICollection<GetSubTaskVM>>
                {
                    Description = $"No task found with ID: {taskId}.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            var data = await _db.SubTasks
                .Where(x => x.TaskId == taskId && !x.IsDeleted)
                .ToListAsync();

            var vm = data.Select(item => new GetSubTaskVM
            {
                Name = item.Name,
                Priority = item.Priority,
                TaskId = item.TaskId,
                Id = item.Id,
                UserId = item.UserId,
            }).ToList();

            Log.Information("Retrieved {Count} SubTasks for TaskId: {TaskId}", vm.Count, taskId);

            return new BaseResponse<ICollection<GetSubTaskVM>>
            {
                Data = vm,
                Description = "SubTasks retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving SubTasks for TaskId: {TaskId}", taskId);

            return new BaseResponse<ICollection<GetSubTaskVM>>
            {
                Description = "An error occurred while retrieving the SubTasks.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetSubTaskVM>>> GetByTaskDone(long taskId)
    {
        try
        {
            var doneTasks = await _db.SubTasks
                .Where(x => x.TaskId == taskId && !x.IsDeleted && x.IsCompleted)
                .ToListAsync();

            if (doneTasks == null || !doneTasks.Any())
            {
                return new BaseResponse<ICollection<GetSubTaskVM>>
                {
                    Description = $"No completed SubTasks found for Task ID: {taskId}.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            var vm = doneTasks.Select(item => new GetSubTaskVM
            {
                Name = item.Name,
                Priority = item.Priority,
                TaskId = item.TaskId,
                Id = item.Id,
                UserId = item.UserId,
            }).ToList();

            Log.Information("Retrieved {Count} completed SubTasks for Task ID: {TaskId}", vm.Count, taskId);

            return new BaseResponse<ICollection<GetSubTaskVM>>
            {
                Data = vm,
                Description = "Completed SubTasks retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving completed SubTasks for Task ID: {TaskId}", taskId);
            return new BaseResponse<ICollection<GetSubTaskVM>>
            {
                Description = "An error occurred while retrieving completed SubTasks.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetSubTaskVM>>> GetByTaskNotDone(long taskId)
    {
        try
        {
            var notDoneTasks = await _db.SubTasks
                .Where(x => x.TaskId == taskId && !x.IsDeleted && !x.IsCompleted)
                .ToListAsync();

            if (notDoneTasks == null || !notDoneTasks.Any())
            {
                return new BaseResponse<ICollection<GetSubTaskVM>>
                {
                    Description = $"No pending SubTasks found for Task ID: {taskId}.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }


            var vm = notDoneTasks.Select(item => new GetSubTaskVM
            {
                Name = item.Name,
                Priority = item.Priority,
                TaskId = item.TaskId,
                Id = item.Id,
                UserId = item.UserId,
            }).ToList();

            Log.Information("Retrieved {Count} pending SubTasks for Task ID: {TaskId}", vm.Count, taskId);

            return new BaseResponse<ICollection<GetSubTaskVM>>
            {
                Data = vm,
                Description = "Pending SubTasks retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving pending SubTasks for Task ID: {TaskId}", taskId);
            return new BaseResponse<ICollection<GetSubTaskVM>>
            {
                Description = "An error occurred while retrieving pending SubTasks.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetSubTaskVM>> Complete(long id)
    {
        try
        {
            var subTask = await _db.SubTasks.SingleOrDefaultAsync(x => x.Id == id);
            if (subTask == null)
            {
                return new BaseResponse<GetSubTaskVM>
                {
                    Description = $"No SubTask found with ID: {id}.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            subTask.IsCompleted = true;
            _db.SubTasks.Update(subTask);
            await _db.SaveChangesAsync();

            var vm = new GetSubTaskVM
            {
                Name = subTask.Name,
                Priority = subTask.Priority,
                TaskId = subTask.TaskId,
                Id = subTask.Id,
                UserId = subTask.UserId,
            };

            Log.Information("SubTask ID: {Id} marked as completed.", id);

            return new BaseResponse<GetSubTaskVM>
            {
                Data = vm,
                Description = "SubTask marked as completed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while marking SubTask ID: {Id} as completed.", id);
            return new BaseResponse<GetSubTaskVM>
            {
                Description = "An error occurred while marking the SubTask as completed.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetSubTaskVM>> Remove(long id)
    {
        try
        {
            var subTaskExist = await _db.SubTasks.SingleOrDefaultAsync(x => x.Id == id);
            if (subTaskExist == null)
            {
                return new BaseResponse<GetSubTaskVM>
                {
                    Description = $"No SubTask found with ID: {id}.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            subTaskExist.IsDeleted = true;
            subTaskExist.DeletedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            var vm = new GetSubTaskVM()
            {
                Name = subTaskExist.Name,
                Priority = subTaskExist.Priority,
                TaskId = subTaskExist.TaskId,
                UserId = subTaskExist.UserId,
                Id = subTaskExist.Id
            };

            Log.Information("SubTask '{SubTaskName}' removed successfully with ID: {Id}", vm.Name, id);

            return new BaseResponse<GetSubTaskVM>
            {
                Data = vm,
                Description = "SubTask removed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing SubTask with ID: {Id}", id);

            return new BaseResponse<GetSubTaskVM>
            {
                Description = "An error occurred while removing the SubTask.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

}
