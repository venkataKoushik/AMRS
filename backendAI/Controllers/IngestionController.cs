using backendAI.Agents;
using backendAI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backendAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngestionController(IngestionAgent ingestionAgent) : ControllerBase
    {
        [HttpPost("upload-and-ingest")]
        [Consumes("multipart/form-data")] // This enables the "Choose File" button in Swagger
        public async Task<ActionResult<IngestionResult>> UploadResearchPaper(IFormFile file)
        {
            // 1. Safety check
            if (file == null || file.Length == 0) return BadRequest("No file selected.");

            // 2. Create a folder on YOUR backend to store the file
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // 3. Combine folder path with the original file name
            var serverFilePath = Path.Combine(folderPath, file.FileName);

            // 4. Save the actual file content to that location
            using (var stream = new FileStream(serverFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Now you have a valid path to give to your IngestionAgent!
            var result = await ingestionAgent.IngestAsync(serverFilePath);

            return Ok(result);
        }
    }
}
