namespace backendAI.Models
{
    public class IngestionResult
    {
        public string DocumentId { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public long FileSizeBytes { get; set; }

        public string FileType { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        
    }
}
