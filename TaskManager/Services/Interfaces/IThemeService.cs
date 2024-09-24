using TaskManager.Models;
using TaskManager.Response;
using TaskManager.ViewModels.Themes;

namespace TaskManager.Services.Interfaces;

public interface IThemeService
{
    public Task<IBaseResponse<Themes>> Create(ThemeVM task);
    public Task<IBaseResponse<ICollection<ThemeVM>>> GetAll();
    public Task<IBaseResponse<ICollection<ThemeVM>>> GetByUser(long id);
    public Task<IBaseResponse<ThemeVM>> GetById(long id);
    public Task<IBaseResponse<Themes>> Remove(long id);
}
