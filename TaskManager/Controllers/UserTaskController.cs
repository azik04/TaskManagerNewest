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
        public async Task<IActionResult> AddUserToTask(long taskId, int userId)
        {
            var result = await _service.AddUserToTask(taskId, userId);
            if (result)
                return Ok("User added to task.");
            return BadRequest("Failed to add user to task.");
        }

        [HttpDelete("{taskId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromTask(long taskId, int userId)
        {
            var result = await _service.RemoveUserFromTask(taskId, userId);
            if (result)
                return Ok("User removed from task.");
            return BadRequest("Failed to remove user from task.");
        }

        [HttpGet("{taskId}/users")]
        public async Task<IActionResult> GetUsersByTaskId(long taskId)
        {
            var users = await _service.GetUsersByTaskId(taskId);
            return Ok(users);
        }
    }
}
