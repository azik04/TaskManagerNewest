using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _db;
        public CommentService(ApplicationDbContext db) {
            _db = db; }
        public async Task<IBaseResponse<Comment>> Create(CreateCommentVM comment)
        {
            try
            {
                var com = new Comment()
                {
                    Message = comment.Message,
                    TaskId = comment.TaskId,
                    UserName = comment.UserName
                };
                await _db.Comments.AddAsync(com);
                await _db.SaveChangesAsync();
                return new BaseResponse<Comment>()
                {
                    Data = com,
                    Description = $"Comment:{com.Id} has been successfully Create",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex) 
            {
                return new BaseResponse<Comment>()
                {
                    Description = ex.Message,
                    StatusCode = Enum.StatusCode.Error
                };
            }
        }

        public async Task<IBaseResponse<ICollection<Comment>>> GetByTask(int taskId)
        {
            try
            {
                var comments = await _db.Comments
                    .Where(c => c.TaskId == taskId)
                    .ToListAsync();

                return new BaseResponse<ICollection<Comment>>
                {
                    Data = comments,
                    Description = "Comments retrieved successfully.",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ICollection<Comment>>
                {
                    Description = ex.Message,
                    StatusCode = Enum.StatusCode.Error
                };
            }
        }

        public async Task<IBaseResponse<Comment>> Remove(int id)
        {
            try
            {
                var con = await _db.Comments.SingleOrDefaultAsync(x => x.Id == id);
                _db.Comments.Remove(con);
                await _db.SaveChangesAsync();
                return new BaseResponse<Comment>
                {
                    Data = con,
                    Description = "Comments remove successfully.",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Comment>()
                {
                    Description = ex.Message,
                    StatusCode = Enum.StatusCode.Error
                };
            }
        }
    }
}
