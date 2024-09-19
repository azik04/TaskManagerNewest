using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Comments;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service) // Change here
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByTask(int id)
    {
        var res = await _service.GetByTask(id); // Await the async call
        return Ok(res);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(int id)
    {
        var res = await _service.Remove(id);
        if (res.StatusCode == TaskManager.Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentVM comment)
    {
        var res = await _service.Create(comment); // Await the async call
        return Ok(res);
    }
}
