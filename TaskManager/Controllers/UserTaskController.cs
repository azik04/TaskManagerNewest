using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Implementations;
using TaskManager.Services.Interfaces;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserThemeService _service;

        public UserTaskController(IUserThemeService service)
        {
            _service = service;
        }
        [HttpPost("{themeId}/users/{userId}")]
        public async Task<IActionResult> AddUserToTask(long themeId, long userId)
        {
            var result = await _service.AddUsersToTask(themeId, userId);
            if (result)
                return Ok("User added to task.");
            return BadRequest("Failed to add user to task.");
        }

        [HttpDelete("{themeId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromTask(long themeId, long userId)
        {
            var result = await _service.RemoveUserFromTheme(themeId, userId);
            if (result)
                return Ok("User removed from task.");
            return BadRequest("Failed to remove user from task.");
        }

        [HttpGet("{themeId}/users")]
        public async Task<IActionResult> GetUsersByThemeId(long themeId)
        {
            var users = await _service.GetUsersByThemeId(themeId);
            return Ok(users);
        }
        [HttpGet("{userId}/theme")]
        public async Task<IActionResult> GetThemeByUserId(long userId)
        {
            var users = await _service.GetThemesByUserId(userId);
            return Ok(users);
        }
    }
}
