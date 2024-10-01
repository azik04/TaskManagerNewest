using TaskManager.Response;
using TaskManager.ViewModels.RegisterVM;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Services.Interfaces;

public interface IUserService
{
    public Task<IBaseResponse<GetUserVM>> Register(RegisterVM task);
    public Task<IBaseResponse<string>> LogIn(LogInVM task);
    public Task<IBaseResponse<ICollection<GetUserVM>>> GetAll();
    public Task<IBaseResponse<ICollection<GetUserVM>>> GetAllUsers();
    public Task<IBaseResponse<ICollection<GetUserVM>>> GetAllAdmins();
    public Task<IBaseResponse<GetUserVM>> Remove(long id);
    public Task<IBaseResponse<GetUserVM>> GetById(long id);
    public Task<IBaseResponse<GetUserVM>> ChangeRole(long id);
    public Task<IBaseResponse<GetUserVM>> ChangePassword(long userId, ChangePasswordVM changePassword);
    public Task<IBaseResponse<GetUserVM>> ChangeEmail(long userId, ChangeEmailVM changePassword);
}
