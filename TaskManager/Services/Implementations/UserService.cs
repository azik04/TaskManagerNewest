using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.RegisterVM;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
    {
        _db = db;
        _contextAccessor = contextAccessor;
    }

    public async Task<IBaseResponse<GetUserVM>> ChangeEmail(long id, ChangeEmailVM changeEmail)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during email change", id);
                return new BaseResponse<GetUserVM>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            if (changeEmail.OldEmail != user.Email)
            {
                Log.Warning("Old email for UserId {UserId} does not match", id);
                return new BaseResponse<GetUserVM>
                {
                    Description = "Old email is incorrect.",
                    StatusCode = Enum.StatusCode.Error
                };
            }

            user.Email = changeEmail.NewEmail;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            var vm = new GetUserVM()
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                Role = user.Role,
            };
            Log.Information("User with Id {UserId} changed email successfully", id);

            return new BaseResponse<GetUserVM>
            {
                Data = vm,
                Description = "Email changed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while changing email for UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<GetUserVM>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetUserVM>> ChangePassword(long id, ChangePasswordVM changePassword)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during password change", id);
                return new BaseResponse<GetUserVM>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            if (changePassword.OldPassword != user.Password)
            {
                Log.Warning("Old password for UserId {UserId} is incorrect", id);
                return new BaseResponse<GetUserVM>
                {
                    Description = "Old password is incorrect.",
                    StatusCode = Enum.StatusCode.Error
                };
            }

            user.Password = changePassword.NewPassword;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            var vm = new GetUserVM()
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                Role = user.Role,
            };
            Log.Information("User with Id {UserId} changed password successfully", id);

            return new BaseResponse<GetUserVM>
            {
                Data = vm,
                Description = "Password changed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while changing password for UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<GetUserVM>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetUserVM>> ChangeRole(long id)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during role change", id);
                return new BaseResponse<GetUserVM>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            user.Role = Enum.Role.Admin;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            var vm = new GetUserVM()
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                Role = user.Role,
            };
            Log.Information("User with Id {UserId} role changed to Admin successfully", id);

            return new BaseResponse<GetUserVM>
            {
                Data = vm,
                Description = "User role changed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while changing role for UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<GetUserVM>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetUserVM>>> GetUnassignedUsersForTask(long taskId)
    {
        try
        {
            var taskAssignedUsers = await _db.UserTasks
                .Where(ut => ut.TaskId == taskId && !ut.IsDeleted)
                .Select(ut => ut.UserId)
                .Distinct()
                .ToListAsync();

            var data = await _db.Users
                .Where(x => !x.IsDeleted && !taskAssignedUsers.Contains(x.Id))
                .ToListAsync();

            Log.Information("Unassigned users for task {TaskId} retrieved successfully", taskId);
            
            var usersVM = data.Select(item => new GetUserVM
            {
                Id = item.Id,
                Email = item.Email,
                Password = item.Password,
                UserName = item.UserName,
                Role = item.Role,
            }).ToList();

            return new BaseResponse<ICollection<GetUserVM>>
            {
                Data = usersVM,
                Description = $"Users successfully retrieved for task {taskId}",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving unassigned users for task {TaskId}: {Message}", taskId, ex.Message);
            return new BaseResponse<ICollection<GetUserVM>>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }



    public async Task<IBaseResponse<ICollection<GetUserVM>>> GetAllAdmins()
    {
        try
        {
            var data = await _db.Users
                .Where(x => !x.IsDeleted && x.Role == Enum.Role.Admin)
                .ToListAsync();

            Log.Information("Successfully retrieved {Count} admin users", data.Count);

            var usersVM = data.Select(item => new GetUserVM
            {
                Id = item.Id,
                Email = item.Email,
                Password = item.Password, 
                UserName = item.UserName,
                Role = item.Role,
            }).ToList();

            return new BaseResponse<ICollection<GetUserVM>>
            {
                Data = usersVM,
                Description = "Admin users retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving admin users: {Message}", ex.Message);
            return new BaseResponse<ICollection<GetUserVM>>
            {
                Description = "An error occurred while retrieving admin users.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetUserVM>>> GetAllUsers()
    {
        try
        {
            var data = await _db.Users
                .Where(x => !x.IsDeleted && x.Role == Enum.Role.User)
                .ToListAsync();

            Log.Information("Successfully retrieved {Count} regular users", data.Count);

            var usersVM = data.Select(item => new GetUserVM
            {
                Id = item.Id,
                Email = item.Email,
                Password = item.Password, 
                UserName = item.UserName,
                Role = item.Role,
            }).ToList();

            return new BaseResponse<ICollection<GetUserVM>>
            {
                Data = usersVM,
                Description = "Regular users retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving regular users: {Message}", ex.Message);
            return new BaseResponse<ICollection<GetUserVM>>
            {
                Description = "An error occurred while retrieving regular users.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }


    public async Task<IBaseResponse<GetUserVM>> GetById(long id)
    {
        try
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during removal", id);
                return new BaseResponse<GetUserVM>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            var vm = new GetUserVM
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                Role = user.Role,

            };
            Log.Information("User with Id {UserId} removed successfully", id);

            return new BaseResponse<GetUserVM>
            {
                Data = vm,
                Description = "User removed successfully",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while removing UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<GetUserVM>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<string>> LogIn(LogInVM task)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == task.UserName && !x.IsDeleted);
            if (user == null || task.Password != user.Password)
            {
                Log.Warning("Login failed for UserName {UserName}: Invalid username or password", task.UserName);
                return new BaseResponse<string>
                {
                    Description = "Username or Password is wrong",
                    StatusCode = Enum.StatusCode.Error
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("0BD0E95C-6387-4135-A80E-489FF6E5C1DF");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Log.Information("User with UserName {UserName} logged in successfully", task.UserName);

            return new BaseResponse<string>
            {
                Description = "Login successful",
                StatusCode = Enum.StatusCode.OK,
                Data = tokenString
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred during login for UserName {UserName}: {Message}", task.UserName, ex.Message);
            return new BaseResponse<string>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetUserVM>> Register(RegisterVM task)
    {
        try
        {
            var existingUser = await _db.Users.SingleOrDefaultAsync(x => x.Email == task.Email);
            if (existingUser != null)
            {
                Log.Warning("Email, {Email} has already been used", existingUser.Email);
                return new BaseResponse<GetUserVM>
                {
                    Description = "Email has already been used.",
                    StatusCode = Enum.StatusCode.Error,
                };
            }

            var user = new Users()
            {
                UserName = task.UserName,
                Password = task.Password,
                Email = task.Email,
                CreateAt = DateTime.Now,
                Role = Enum.Role.User
            };
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            var vm = new GetUserVM
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                Role = user.Role,
            };
            Log.Information("User with UserName {UserName} registered successfully", task.UserName);

            return new BaseResponse<GetUserVM>()
            {
                Data = vm,
                Description = "User registered successfully",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while registering UserName {UserName}: {Message}", task.UserName, ex.Message);
            return new BaseResponse<GetUserVM>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<GetUserVM>> Remove(long id)
    {
        try
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during removal", id);
                return new BaseResponse<GetUserVM>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.Now;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            var vm = new GetUserVM
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                Role = user.Role,

            };
            Log.Information("User with Id {UserId} removed successfully", id);

            return new BaseResponse<GetUserVM>
            {
                Data = vm,
                Description = "User removed successfully",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while removing UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<GetUserVM>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<GetUserVM>>> GetAll()
    {
        try
        {
            var data = await _db.Users
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            Log.Information("Successfully retrieved {Count} regular users", data.Count);

            var usersVM = data.Select(item => new GetUserVM
            {
                Id = item.Id,
                Email = item.Email,
                Password = item.Password,
                UserName = item.UserName,
                Role = item.Role,
            }).ToList();

            return new BaseResponse<ICollection<GetUserVM>>
            {
                Data = usersVM,
                Description = "Regular users retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving regular users: {Message}", ex.Message);
            return new BaseResponse<ICollection<GetUserVM>>
            {
                Description = "An error occurred while retrieving regular users.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }
}

