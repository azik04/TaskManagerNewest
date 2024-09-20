using TaskManager.Models;
using TaskManager.Response;
using TaskManager.ViewModels.Users;

namespace TaskManager.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IBaseResponse<Users>> Register(AccountVM task);
        public Task<IBaseResponse<string>> LogIn(LogInVM task);
        public Task<IBaseResponse<ICollection<Users>>> GetAll();
        public Task<IBaseResponse<Users>> Remove(long id);
        public Task<IBaseResponse<Users>> ChangeRole(long id);
        public Task<IBaseResponse<Users>> ChangePassword(long userId, ChangePasswordVM changePassword);
        public Task<IBaseResponse<Users>> ChangeEmail(long userId, ChangeEmail changePassword);
    }
}
