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

    public async Task<IBaseResponse<Comment>> Create(CreateCommentVM comment)
    {
        try
        {
            var com = new Comment()
            {
                Message = comment.Message,
                TaskId = comment.TaskId,
                UserName = comment.UserName,
                CreateAt = DateTime.Now,
            };

            await _db.Comments.AddAsync(com);
            await _db.SaveChangesAsync();

            Log.Information("Comment with Id {CommentId} has been successfully created.", com.Id);

            return new BaseResponse<Comment>()
            {
                Data = com,
                Description = $"Comment:{com.Id} has been successfully created.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating comment: {Message}", ex.Message);
            return new BaseResponse<Comment>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<Comment>>> GetByTask(long taskId)
    {
        try
        {
            var comments = await _db.Comments
                .Where(c => c.TaskId == taskId && !c.IsDeleted)
                .ToListAsync();

            Log.Information("Retrieved {CommentCount} comments for task with Id {TaskId}.", comments.Count, taskId);

            return new BaseResponse<ICollection<Comment>>
            {
                Data = comments,
                Description = "Comments retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving comments for task with Id {TaskId}: {Message}", taskId, ex.Message);
            return new BaseResponse<ICollection<Comment>>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<Comment>> Remove(long id)
    {
        try
        {
            var con = await _db.Comments.SingleOrDefaultAsync(x => x.Id == id);
            if (con == null)
            {
                Log.Warning("Attempted to remove a comment with Id {CommentId} that does not exist.", id);
                return new BaseResponse<Comment>
                {
                    Description = "Comment not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }
            con.DeletedAt = DateTime.Now;
            con.IsDeleted = true;
            _db.Comments.Remove(con);
            await _db.SaveChangesAsync();

            Log.Information("Comment with Id {CommentId} has been removed successfully.", id);
            return new BaseResponse<Comment>
            {
                Data = con,
                Description = "Comment removed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing comment with Id {CommentId}: {Message}", id, ex.Message);
            return new BaseResponse<Comment>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }
}
