using System.Text.Json.Serialization;

namespace TaskManager.Models
{
    public class Files
    {
        public long Id { get; set; }
        public string FileName { get; set; } // Changed to FileName for clarity

        // Foreign key for Task
        public long TaskId { get; set; }
        public Tasks Task { get; set; } // Navigation property

        public bool IsDeleted { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
