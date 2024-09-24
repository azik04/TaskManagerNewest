using Microsoft.AspNetCore.Mvc;
using System.IO;
using TaskManager.Models;
using TaskManager.Response;

namespace TaskManager.Services.Interfaces;

public interface IFileService
{
    Task<bool> UploadFile(Stream fileStream, long taskId, string fileName);
    Task<FileStreamResult> DownloadFile(long id);
    Task<bool> DeleteFile(long id);
    Task<BaseResponse<ICollection<Files>>> ListFilesAsync(long taskId);
}
