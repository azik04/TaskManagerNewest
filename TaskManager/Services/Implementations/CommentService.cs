using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _db;

    public CommentService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IBaseResponse<GetCommentVM>> Create(CreateCommentVM comment)
    {
        try
        {
            // Validate TaskId
            var taskExists = await _db.Tasks.AnyAsync(t => t.Id == comment.TaskId);
            if (!taskExists)
            {
                return new BaseResponse<GetCommentVM>
                {
                    Description = "Invalid TaskId.",
                    StatusCode = Enum.StatusCode.Error
                };
            }

            // Validate UserId
            var userExists = await _db.Users.AnyAsync(u => u.Id == comment.UserId);
            if (!userExists)
            {
                return new BaseResponse<GetCommentVM>
                {
                    Description = "Invalid UserId.",
                    StatusCode = Enum.StatusCode.Error
                };
            }

            var com = new Comments()
            {
                Message = comment.Message,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                CreateAt = DateTime.Now,
            };

            await _db.Comments.AddAsync(com);
            await _db.SaveChangesAsync();

            var vm = new GetCommentVM()
            {
                Id = com.Id,
                Message = com.Message,
                TaskId = com.TaskId,
                UserId = com.UserId,
                CreateAt = com.CreateAt
            };
            Log.Information("Comment with Id {CommentId} has been successfully created.", com.Id);

            return new BaseResponse<GetCommentVM>()
            {
                Data = vm,
                Description = $"Comment: {com.Id} has been successfully created.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (DbUpdateException dbEx)
        {
            Log.Error(dbEx, "Database update error: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
            return new BaseResponse<GetCommentVM>
            {
                Description = dbEx.InnerException?.Message ?? dbEx.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating comment: {Message}", ex.Message);
            return new BaseResponse<GetCommentVM>()
            {
                Description = ex.InnerException?.Message ?? ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }


    public async Task<IBaseResponse<ICollection<GetCommentVM>>> GetByTask(long taskId)
    {
        try
        {
            var comments = await _db.Comments
                .Where(c => c.TaskId == taskId && !c.IsDeleted)
                .ToListAsync();

            var commentVMs = comments.Select(item => new GetCommentVM
            {
                Id = item.Id,
                Message = item.Message,
                UserId = item.UserId,
                TaskId = item.TaskId,
                CreateAt = item.CreateAt
            }).ToList();

            Log.Information("Retrieved {CommentCount} comments for task with Id {TaskId}.", comments.Count, taskId);

            return new BaseResponse<ICollection<GetCommentVM>>
            {
                Data = commentVMs,
                Description = "Comments retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving comments for task with Id {TaskId}: {Message}", taskId, ex.Message);
            return new BaseResponse<ICollection<GetCommentVM>>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetCommentVM>> Remove(long id)
    {
        try
        {
            var con = await _db.Comments.SingleOrDefaultAsync(x => x.Id == id);
            if (con == null)
            {
                Log.Warning("Attempted to remove a comment with Id {CommentId} that does not exist.", id);
                return new BaseResponse<GetCommentVM>
                {
                    Description = "Comment not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            con.DeletedAt = DateTime.Now;
            con.IsDeleted = true;
            _db.Comments.Update(con);
            await _db.SaveChangesAsync();

            var vm = new GetCommentVM()
            {
                Id = con.Id,
                Message = con.Message,
                TaskId = con.TaskId,
                UserId = con.UserId,
                CreateAt = con.CreateAt

            };

            Log.Information("Comment with Id {CommentId} has been removed successfully.", id);
            return new BaseResponse<GetCommentVM>
            {
                Data = vm,
                Description = "Comment removed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing comment with Id {CommentId}: {Message}", id, ex.Message);
            return new BaseResponse<GetCommentVM>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }
}
