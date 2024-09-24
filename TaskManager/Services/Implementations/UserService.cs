﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Users;

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

    public async Task<IBaseResponse<Users>> ChangeEmail(long id, ChangeEmail changeEmail)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during email change", id);
                return new BaseResponse<Users>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            if (changeEmail.OldEmail != user.Email)
            {
                Log.Warning("Old email for UserId {UserId} does not match", id);
                return new BaseResponse<Users>
                {
                    Description = "Old email is incorrect.",
                    StatusCode = Enum.StatusCode.Error
                };
            }

            user.Email = changeEmail.NewEmail;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            Log.Information("User with Id {UserId} changed email successfully", id);

            return new BaseResponse<Users>
            {
                Data = user,
                Description = "Email changed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while changing email for UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<Users>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<Users>> ChangePassword(long id, ChangePasswordVM changePassword)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during password change", id);
                return new BaseResponse<Users>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            if (changePassword.OldPassword != user.Password)
            {
                Log.Warning("Old password for UserId {UserId} is incorrect", id);
                return new BaseResponse<Users>
                {
                    Description = "Old password is incorrect.",
                    StatusCode = Enum.StatusCode.Error
                };
            }

            user.Password = changePassword.NewPassword;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            Log.Information("User with Id {UserId} changed password successfully", id);

            return new BaseResponse<Users>
            {
                Data = user,
                Description = "Password changed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while changing password for UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<Users>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<Users>> ChangeRole(long id)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during role change", id);
                return new BaseResponse<Users>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            user.Role = Enum.Role.Admin;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            Log.Information("User with Id {UserId} role changed to Admin successfully", id);

            return new BaseResponse<Users>
            {
                Data = user,
                Description = "User role changed successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while changing role for UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<Users>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<Users>>> GetAll()
    {
        try
        {
            var data = await _db.Users.ToListAsync();
            Log.Information("All users retrieved successfully");

            return new BaseResponse<ICollection<Users>>
            {
                Data = data,
                Description = "Users successfully retrieved",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving all users: {Message}", ex.Message);
            return new BaseResponse<ICollection<Users>>
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
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == task.UserName && !x.isDeleted);
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
            var key = Encoding.ASCII.GetBytes("YourVeryStrongSecretKey123456789012");
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

    public async Task<IBaseResponse<Users>> Register(AccountVM task)
    {
        try
        {
            var data = new Users()
            {
                UserName = task.UserName,
                Password = task.Password,
                Email = task.Email,
                Role = Enum.Role.User
            };
            await _db.Users.AddAsync(data);
            await _db.SaveChangesAsync();

            Log.Information("User with UserName {UserName} registered successfully", task.UserName);

            return new BaseResponse<Users>()
            {
                Data = data,
                Description = "User registered successfully",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while registering UserName {UserName}: {Message}", task.UserName, ex.Message);
            return new BaseResponse<Users>()
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<Users>> Remove(long id)
    {
        try
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                Log.Warning("User with Id {UserId} not found during removal", id);
                return new BaseResponse<Users>
                {
                    Description = "User not found.",
                    StatusCode = Enum.StatusCode.NotFound
                };
            }

            user.isDeleted = true;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            Log.Information("User with Id {UserId} removed successfully", id);

            return new BaseResponse<Users>
            {
                Data = user,
                Description = "User removed successfully",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while removing UserId {UserId}: {Message}", id, ex.Message);
            return new BaseResponse<Users>
            {
                Description = ex.Message,
                StatusCode = Enum.StatusCode.Error
            };
        }
    }
}
