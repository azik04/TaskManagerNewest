﻿using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.Comments;

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
    public async Task<IActionResult> GetByTask(long id)
    {
        var res = await _service.GetByTask(id);
        return Ok(res);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        var res = await _service.Remove(id);
        if (res.StatusCode == TaskManager.Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }

    [HttpPost]
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
