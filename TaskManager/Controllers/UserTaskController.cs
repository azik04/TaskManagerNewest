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
        [HttpPost("{taskId}/users/{userId}")]
        public async Task<IActionResult> AddUserToTask(long taskId, long userId)
        {
            var result = await _service.AddUsersToTask(taskId, userId);
            if (result)
                return Ok("User added to task.");
            return BadRequest("Failed to add user to task.");
        }

        [HttpDelete("{taskId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromTask(long taskId, long userId)
        {
            var result = await _service.RemoveUserFromTheme(taskId, userId);
            if (result)
                return Ok("User removed from task.");
            return BadRequest("Failed to remove user from task.");
        }

        [HttpGet("{taskId}/users")]
        public async Task<IActionResult> GetUsersByTaskId(long taskId)
        {
            var users = await _service.GetUsersByThemeId(taskId);
            return Ok(users);
        }
        [HttpGet("{userId}/theme")]
        public async Task<IActionResult> GetTaskByUserId(long userId)
        {
            var users = await _service.GetThemesByUserId(userId);
            return Ok(users);
        }
    }
}
