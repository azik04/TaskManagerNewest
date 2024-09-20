using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Threading.Tasks;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels;
using TaskManager.ViewModels.Tasks;

namespace TaskManager.Controllers
{
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
        public IActionResult GetAllNotDone(long themeId)
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

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(long fileId)
        {
            var fileEntity = await _db.Files.FindAsync(fileId);
            if (fileEntity == null || fileEntity.IsDeleted)
            {
                return NotFound();
            }

            var filePath = Path.Combine("wwwroot", "upload", fileEntity.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType)
                ? mimeType
                : "application/octet-stream";

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, contentType, fileEntity.FileName);
        }
    }
}
