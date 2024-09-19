using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting; // Ensure you have this
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Context;
using TaskManager.Models;
using TaskManager.Response;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHostEnvironment _env; // Use IHostEnvironment here

        public FileService(ApplicationDbContext dbContext, IHostEnvironment env) // Constructor
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

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload failed: {ex.Message}");
                return false;
            }
        }

        public async Task<FileStreamResult> DownloadFile(long id)
        {
            try
            {
                var fileRecord = await _dbContext.Files.FindAsync(id);
                if (fileRecord == null || fileRecord.IsDeleted)
                    return null;

                var localFilePath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", fileRecord.FileName);
                var stream = new FileStream(localFilePath, FileMode.Open);

                return new FileStreamResult(stream, "application/octet-stream")
                {
                    FileDownloadName = fileRecord.FileName
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteFile(long id)
        {
            try
            {
                var fileRecord = await _dbContext.Files.FindAsync(id);
                if (fileRecord == null)
                    return false;

                fileRecord.IsDeleted = true;
                _dbContext.Files.Update(fileRecord);
                await _dbContext.SaveChangesAsync();

                var localFilePath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", fileRecord.FileName);
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deletion failed: {ex.Message}");
                return false;
            }
        }

        public async Task<BaseResponse<ICollection<Files>>> ListFilesAsync(long taskId)
        {
            var files = await _dbContext.Files
                .Where(f => f.TaskId == taskId && !f.IsDeleted)
                .ToListAsync();

            return new BaseResponse<ICollection<Files>>
            {
                Data = files,
                Description = "Files retrieved successfully.",
                StatusCode = Enum.StatusCode.OK
            };
        }
    }
}
