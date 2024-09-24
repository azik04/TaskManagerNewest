using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels;

namespace TaskManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileVM uploadFile)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (uploadFile.File == null || uploadFile.TaskId <= 0)
            return BadRequest("Invalid file or task ID.");

        var result = await _fileService.UploadFile(uploadFile.File.OpenReadStream(), uploadFile.TaskId, uploadFile.File.FileName);
        if (result)
            return Ok("File uploaded successfully.");

        return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadFile(long id)
    {
        var fileResult = await _fileService.DownloadFile(id);
        if (fileResult == null)
            return NotFound("File not found.");

        return fileResult; 
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(long id)
    {
        var result = await _fileService.DeleteFile(id);
        if (result)
            return Ok("File deleted successfully.");

        return NotFound("File not found or could not be deleted.");
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> ListFiles(long taskId)
    {
        var files = await _fileService.ListFilesAsync(taskId);
        return Ok(files);
    }
}
