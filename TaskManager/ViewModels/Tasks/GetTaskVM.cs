using TaskManager.ViewModels.Files;
using TaskManager.ViewModels.Themes;

namespace TaskManager.ViewModels.Tasks
{
    public class GetTaskVM
    {
        public long Id { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string DeadLine { get; set; }
        public DateTime DateOfCompletion { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<FilesVM> Files { get; set; }
        public long ThemeId { get; set; }
    }
}
