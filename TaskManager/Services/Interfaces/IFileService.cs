using Microsoft.AspNetCore.Mvc;
using TaskManager.Response;
using TaskManager.ViewModels.Files;

namespace TaskManager.Services.Interfaces;

public interface IFileService
{
    Task<bool> UploadFile(Stream fileStream, long taskId, string fileName);
    Task<FileStreamResult> DownloadFile(long id);
    Task<bool> DeleteFile(long id);
    Task<BaseResponse<ICollection<GetFileVM>>> ListFilesAsync(long taskId);
}
