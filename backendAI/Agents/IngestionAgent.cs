using backendAI.Models;
using System.Diagnostics;

namespace backendAI.Agents
{
    public class IngestionAgent
    {
        private readonly ILogger<IngestionAgent> _logger;
        private readonly IConfiguration _config;

        private string WatchFolder =>
            _config["Ingestion:WatchFolder"] ?? Path.Combine(Path.GetTempPath(), "Documents");

        public IngestionAgent(ILogger<IngestionAgent> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            if (!Directory.Exists(WatchFolder))
            {
                Directory.CreateDirectory(WatchFolder);
            }
        }

        public async Task<IngestionResult> IngestAsync(string filePath)
        {
            var sw = Stopwatch.StartNew();
            var fileInfo = new FileInfo(filePath);

            // Generate unique identifiers based on project requirements
            string fileNameOnly = Path.GetFileNameWithoutExtension(fileInfo.Name);
            string firstName = fileNameOnly.Length >= 2 ? fileNameOnly.Substring(0, 2) : fileNameOnly;
            string lastName = fileNameOnly.Length >= 2 ? fileNameOnly.Substring(fileNameOnly.Length - 2) : "";
            string timeStamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");

            _logger.LogInformation("[IngestionAgent] Ingesting file '{FilePath}'", filePath);

            await Task.Delay(100);

            // UPDATED: Using Object Initializer to match your new Model
            var result = new IngestionResult
            {
                DocumentId = $"DOC-{firstName}-{lastName}-{timeStamp}",
                FilePath = filePath,
                FileName = fileInfo.Name,
                FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                FileType = fileInfo.Extension.TrimStart('.').ToUpperInvariant(),
                ReceivedAt = DateTime.UtcNow
            };

            sw.Stop();
            _logger.LogInformation(
                "[IngestionAgent] Ingested '{FileName}' ({Size} bytes) in {Ms}ms",
                result.FileName, result.FileSizeBytes, sw.ElapsedMilliseconds);

            return result;
        }

        public async Task<IReadOnlyList<IngestionResult>> ListPendingAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("[IngestionAgent] Scanning watch folder '{Folder}'", WatchFolder);

            await Task.Delay(50, ct);

            var filesList = new List<IngestionResult>();
            string[] files = Directory.GetFiles(WatchFolder);

            foreach (var path in files)
            {
                string ext = Path.GetExtension(path).ToLower();
                // Validating file types as per Pre-process Agent requirements
                if (ext == ".pdf" || ext == ".docx" || ext == ".doc" || ext == ".png" || ext == ".jpg")
                {
                    var fi = new FileInfo(path);
                    string fileNameOnly = Path.GetFileNameWithoutExtension(fi.Name);
                    string firstName = fileNameOnly.Length >= 2 ? fileNameOnly.Substring(0, 2) : fileNameOnly;
                    string lastName = fileNameOnly.Length >= 2 ? fileNameOnly.Substring(fileNameOnly.Length - 2) : "";
                    string timeStamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");

                    // UPDATED: Using Object Initializer
                    filesList.Add(new IngestionResult
                    {
                        DocumentId = $"DOC-{firstName}-{lastName}-{timeStamp}",
                        FilePath = path,
                        FileName = fi.Name,
                        FileSizeBytes = fi.Length,
                        FileType = fi.Extension.TrimStart('.').ToUpperInvariant(),
                        ReceivedAt = fi.CreationTimeUtc
                    });
                }
            }

            _logger.LogInformation("[IngestionAgent] Found {Count} pending document(s)", filesList.Count);
            return filesList;
        }
    }
}