using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Context;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Tasks;

namespace TaskManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _service;
    private readonly ApplicationDbContext _db;
    public TaskController(ITaskService service, ApplicationDbContext db)
    {
        _db = db;
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> Create(CreateTaskVM task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }
        var res = await _service.Create(task);
        if(res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }
    [HttpGet]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> GetAll()
    {
        var res = await _service.GetAll(); 
            return Ok(res);
        
    }
    [HttpGet("done")]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> GetAllDone(long themeId)
    {
        var res = await _service.GetAllDone(themeId);
        return Ok(res);

    }
    [HttpGet("notdone")]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> GetAllNotDone(long themeId)
    {
        var res = await _service.GetAllNotDone(themeId);
        return Ok(res);

    }
    [HttpGet("{id}")]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> GetById(long id)
   {
        var res = await _service.GetById(id);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
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
    [HttpPut]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> Update(long id, UpdateTaskVM task)
    {
        var res = await _service.Update(id, task);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res.Data);

        return BadRequest(res);
    }
    [HttpPut("complite/{id}")]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> Complite(long id)
    {
        var res = await _service.Complite(id);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res.Data);

        return BadRequest(res);
    }

}
