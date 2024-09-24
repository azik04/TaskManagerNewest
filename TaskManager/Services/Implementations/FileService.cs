using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations;

public class FileService : IFileService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHostEnvironment _env;

    public FileService(ApplicationDbContext dbContext, IHostEnvironment env)
    {
        _dbContext = dbContext;
        _env = env;
    }

    public async Task<bool> UploadFile(Stream fileStream, long taskId, string fileName)
    {
        try
        {
            var uploadDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
                Log.Information("Created directory: {UploadDir}", uploadDir);
            }

            var file = new Files
            {
                FileName = fileName,
                TaskId = taskId,
                IsDeleted = false,
                CreateAt = DateTime.UtcNow
            };

            await _dbContext.Files.AddAsync(file);
            await _dbContext.SaveChangesAsync();

            var localFilePath = Path.Combine(uploadDir, fileName);

            using (var fileStreamOutput = new FileStream(localFilePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            Log.Information("Uploaded file: {FileName} for TaskId: {TaskId}", fileName, taskId);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Upload failed for TaskId: {TaskId}, FileName: {FileName}", taskId, fileName);
            return false;
        }
    }

    public async Task<FileStreamResult> DownloadFile(long id)
    {
        try
        {
            var fileRecord = await _dbContext.Files.FindAsync(id);
            if (fileRecord == null || fileRecord.IsDeleted)
            {
                Log.Warning("File not found or deleted for Id: {Id}", id);
                return null;
            }

            var localFilePath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", fileRecord.FileName);
            var stream = new FileStream(localFilePath, FileMode.Open);

            Log.Information("Downloaded file: {FileName} for Id: {Id}", fileRecord.FileName, id);
            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = fileRecord.FileName
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Download failed for Id: {Id}", id);
            return null;
        }
    }

    public async Task<bool> DeleteFile(long id)
    {
        try
        {
            var fileRecord = await _dbContext.Files.FindAsync(id);
            if (fileRecord == null)
            {
                Log.Warning("File not found for Id: {Id}", id);
                return false;
            }

            fileRecord.IsDeleted = true;
            _dbContext.Files.Update(fileRecord);
            await _dbContext.SaveChangesAsync();

            var localFilePath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", fileRecord.FileName);
            if (File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
                Log.Information("Deleted file: {FileName} for Id: {Id}", fileRecord.FileName, id);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Deletion failed for Id: {Id}", id);
            return false;
        }
    }

    public async Task<BaseResponse<ICollection<Files>>> ListFilesAsync(long taskId)
    {
        try
        {
            var files = await _dbContext.Files
                .Where(f => f.TaskId == taskId && !f.IsDeleted)
                .ToListAsync();

            Log.Information("Retrieved {FileCount} files for TaskId: {TaskId}", files.Count, taskId);
            return new BaseResponse<ICollection<Files>>
            {
                Data = files,
                Description = "Files retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving files for TaskId: {TaskId}", taskId);
            return new BaseResponse<ICollection<Files>>
            {
                Description = "Error retrieving files.",
                StatusCode = Enum.StatusCode.Error
            };
        }
    }
}
