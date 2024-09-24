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
    public IActionResult GetAll()
    {
        var res = _service.GetAll(); 
            return Ok(res);
        
    }
    [HttpGet("done")]
    public IActionResult GetAllDone(long themeId)
    {
        var res = _service.GetAllDone(themeId);
        return Ok(res);

    }
    [HttpGet("notdone")]
    public async Task<IActionResult> GetAllNotDone(long themeId)
    {
        var res = _service.GetAllNotDone(themeId);
        return Ok(res);

    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
   {
        var res = await _service.GetById(id);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        var res = await _service.Remove(id);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }
    [HttpPut]
    public async Task<IActionResult> Update(long id, UpdateTaskVM task)
    {
        var res = await _service.Update(id, task);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res.Data);

        return BadRequest(res);
    }
    [HttpPut("complite/{id}")]
    public async Task<IActionResult> Complite(long id)
    {
        var res = await _service.Complite(id);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res.Data);

        return BadRequest(res);
    }

}
