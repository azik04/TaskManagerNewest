using TaskManager.Enum;

namespace TaskManager.ViewModels.UsersVMs;

public class GetUserVM
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}
