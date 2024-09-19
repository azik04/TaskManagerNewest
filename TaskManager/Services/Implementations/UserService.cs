using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TaskManager.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserService(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;
        }

        public async Task<IBaseResponse<Users>> ChangeEmail(int id, ChangeEmail changeEmail)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return new BaseResponse<Users>
                    {
                        Description = "User not found.",
                        StatusCode = Enum.StatusCode.NotFound
                    };
                }

                if (changeEmail.OldEmail != user.Email)
                {
                    return new BaseResponse<Users>
                    {
                        Description = "Old password is incorrect.",
                        StatusCode = Enum.StatusCode.NotFound
                    };
                }

                user.Email = changeEmail.NewEmail;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                return new BaseResponse<Users>
                {
                    Data = user,
                    Description = "Password changed successfully.",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Users>
                {
                    Description = ex.Message,
                    StatusCode = Enum.StatusCode.Error
                };
            }

        }

        public async Task<IBaseResponse<Users>> ChangePassword(int id, ChangePasswordVM changePassword)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return new BaseResponse<Users>
                    {
                        Description = "User not found.",
                        StatusCode = Enum.StatusCode.NotFound
                    };
                }

                if (changePassword.OldPassword != user.Password)
                {
                    return new BaseResponse<Users>
                    {
                        Description = "Old password is incorrect.",
                        StatusCode = Enum.StatusCode.NotFound
                    };
                }

                user.Password = changePassword.NewPassword;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                return new BaseResponse<Users>
                {
                    Data = user,
                    Description = "Password changed successfully.",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Users>
                {
                    Description = ex.Message,
                    StatusCode = Enum.StatusCode.Error
                };
            }

        }


        public async Task<IBaseResponse<Users>> ChangeRole(int id)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
                user.Role = Enum.Role.Admin;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return new BaseResponse<Users>
                {
                    Data = user,
                    Description = "Users successfully retrieved",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
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

                ICollection<Users> usersCollection = data;

                return new BaseResponse<ICollection<Users>>
                {
                    Data = usersCollection,
                    Description = "Users successfully retrieved",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
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
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("UserId", user.Id.ToString())
            }),
                    Expires = DateTime.UtcNow.AddDays(1), 
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new BaseResponse<string>
                {
                    Description = "Login successful",
                    StatusCode = Enum.StatusCode.OK,
                    Data = tokenString
                };
            }
            catch (Exception ex)
            {
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
                return new BaseResponse<Users>()
                {
                    Data = data,
                    Description = "Themes successfully retrieved",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new BaseResponse<Users>()
                {
                    Description = ex.Message,
                    StatusCode = Enum.StatusCode.OK
                };
            }
        }

        public async Task<IBaseResponse<Users>> Remove(int id)
        {
            try
            {
                var data = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
                data.isDeleted = true;
                _db.Users.Remove(data);
                await _db.SaveChangesAsync();
                return new BaseResponse<Users>()
                {
                    Data = data,
                    Description = "Themes successfully retrieved",
                    StatusCode = Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Users>()
                {
                    Description = ex.Message,
                    StatusCode = Enum.StatusCode.OK
                };
            }
        }
    }
}
