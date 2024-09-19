using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TaskManager.Context;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels;
using TaskManager.ViewModels.Themes;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IThemeService _service;
        private readonly ApplicationDbContext _db;
        public ThemeController(IThemeService service, ApplicationDbContext db)
        {
            _db = db;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThemeVM task)
        {
            var res = await _service.Create(task);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res);

            return BadRequest(res);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var res = _service.GetAll();
            return Ok(res);

        }
        [HttpGet("userId/{id}")]
        public IActionResult GetByUser(int id)
       {
            var res = _service.GetByUser(id);
            return Ok(res);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _service.GetById(id);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var res = await _service.Remove(id);
            if (res.StatusCode == Enum.StatusCode.OK)
                return Ok(res);

            return BadRequest(res);
        }
       
    }
}
