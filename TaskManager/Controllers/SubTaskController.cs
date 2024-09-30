using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Context;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.SubTask;

namespace TaskManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubTaskController : ControllerBase
{
    private readonly ISubTaskService _service;
    private readonly ApplicationDbContext _db;
    public SubTaskController(ISubTaskService service, ApplicationDbContext db)
    {
        _db = db;
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Create(CreateSubTaskVM subTask)
     {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var res = await _service.Create(subTask);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }
    [HttpGet("{taskId}")]
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
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }
}
