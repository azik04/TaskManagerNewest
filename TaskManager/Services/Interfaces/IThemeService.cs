using TaskManager.Response;
using TaskManager.ViewModels.Themes;

namespace TaskManager.Services.Interfaces;

public interface IThemeService
{
    public Task<IBaseResponse<GetThemeVM>> Create(CreateThemeVM task);
    public Task<IBaseResponse<ICollection<GetThemeVM>>> GetAll();
    public Task<IBaseResponse<ICollection<GetThemeVM>>> GetByUser(long id);
    public Task<IBaseResponse<GetThemeVM>> GetById(long id);
    public Task<IBaseResponse<GetThemeVM>> Remove(long id);
}
