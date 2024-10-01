using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.UsersVMs;
using TaskManager.ViewModels.RegisterVM;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Register(RegisterVM task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var res = await _service.Register(task);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }
    [HttpPost("login")]
    public async Task<IActionResult> LogIn(LogInVM task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var res = await _service.LogIn(task);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }
    [HttpGet("/{taskId}/GetUnassignedUsers")]
    public async Task<IActionResult> GetAll(long taskId)
    {
        var res = await _service.GetUnassignedUsersForTask(taskId);
        return Ok(res);
    }
    [HttpGet("Admins")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var res = await _service.GetAllAdmins();
        return Ok(res);
    }
    [HttpGet("Users")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var res = await _service.GetAllUsers();
        return Ok(res);
    }
    [HttpGet("{id}")]

    public async Task<IActionResult> GetById(long id)
    {
        var res = await _service.GetById(id);
        return Ok(res);
    }

    [HttpDelete]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Remove(long id)
    {
        var res = await _service.Remove(id);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res);

        return BadRequest(res);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogOut()
    {
        return Ok("LogOut successfully");
    }
    [HttpPut("ChangeRole")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ChangeRole(long id)
    {
        var res = await _service.ChangeRole(id);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res.Data);

        return BadRequest(res);
    }
    [HttpPut("ChangePassword")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ChangePassword(long id, ChangePasswordVM changePassword )
    {
        var res = await _service.ChangePassword(id,  changePassword);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res.Data);

        return BadRequest(res);
    }
    [HttpPut("ChangeEmail")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ChangeEmail(long id, ChangeEmailVM changeEmail)
    {
        var res = await _service.ChangeEmail(id, changeEmail);
        if (res.StatusCode == Enum.StatusCode.OK)
            return Ok(res.Data);

        return BadRequest(res);
    }
}
