using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Themes;
using TaskManager.ViewModels.Users;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AccountVM task)
        {
            var res = await _service.Register(task);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res);

            return BadRequest(res);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInVM task)
        {
            var res = await _service.LogIn(task);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res);

            return BadRequest(res);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var res = _service.GetAll();
            return Ok(res);

        }
        

        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var res = await _service.Remove(id);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        [HttpPut("ChangeRole")]
        public async Task<IActionResult> ChangeRole(int id)
        {
            var res = await _service.ChangeRole(id);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res.Data);

            return BadRequest(res);
        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordVM changePassword )
        {
            var res = await _service.ChangePassword(id,  changePassword);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res.Data);

            return BadRequest(res);
        }
        [HttpPut("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(int id, ChangeEmail changeEmail)
        {
            var res = await _service.ChangeEmail(id, changeEmail);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res.Data);

            return BadRequest(res);
        }
    }
}
