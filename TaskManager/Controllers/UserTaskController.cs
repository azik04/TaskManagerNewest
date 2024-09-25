using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserTaskService _service;

        public UserTaskController(IUserTaskService service)
        {
            _service = service;
        }
        [HttpPost("{taskId}/users/{userId}")]
        [Authorize(Policy = "User")]

        public async Task<IActionResult> AddUserToTask(long taskId, long userId)
        {
            var result = await _service.AddUsersToTask(taskId, userId);
            if (result)
                return Ok("User added to task.");
            return BadRequest("Failed to add user to task.");
        }

        [HttpDelete("{taskId}/users/{userId}")]
        [Authorize(Policy = "User")]

        public async Task<IActionResult> RemoveUserFromTask(long taskId, long userId)
        {
            var result = await _service.RemoveUserFromTask(taskId, userId);
            if (result)
                return Ok("User removed from task.");
            return BadRequest("Failed to remove user from task.");
        }

        [HttpGet("{taskId}/users")]
        [Authorize(Policy = "User")]

        public async Task<IActionResult> GetUsersByTaskId(long taskId)
        {
            var users = await _service.GetUsersByTaskId(taskId);
            return Ok(users);
        }
        [HttpGet("{userId}/theme")]
        [Authorize(Policy = "User")]

        public async Task<IActionResult> GetTaskByUserId(long userId)
        {
            var users = await _service.GetTaskByUserId(userId);
            return Ok(users);
        }
    }
}
