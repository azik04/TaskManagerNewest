using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service) 
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> GetByTask(long taskId)
    {
        var res = await _service.GetByTask(taskId);
        return Ok(res);
    }

    [HttpDelete]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> Remove(long id)
    {
        var res = await _service.Remove(id);
        if (res.StatusCode == TaskManager.Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }

    [HttpPost]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> Create(CreateCommentVM comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var res = await _service.Create(comment);
        return Ok(res);
    }
}
