using backendAI.Agents;
using backendAI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backendAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreProcessController(PreProcessAgent preProcessAgent) : ControllerBase
    {
        [HttpPost("process-file")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PreProcessResult>> PreProcessFile(IFormFile file)
        {
            // 1. Validate that a file was actually sent
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            try
            {
                // 2. Ensure the temporary "Uploads" directory exists
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // 3. Define the path where the file will be stored on the server
                var serverFilePath = Path.Combine(folderPath, file.FileName);

                // 4. Save the uploaded stream to the physical path
                using (var stream = new FileStream(serverFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 5. Call the PreProcessAgent to handle OCR, Text Extraction, and Language Detection
                var result = await preProcessAgent.Process(serverFilePath);

                // 6. Check if the agent logic succeeded (e.g., supported file types)
                if (!result.Success)
                {
                    return BadRequest(result.Error);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
