using backendAI.Agents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel; // Add this

namespace backendAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : Controller 
    {
        private readonly TranslationAgent _translationAgent;

      
       public TranslationController(Kernel kernel)
        {
            _translationAgent = new TranslationAgent(kernel);
        }

        [HttpGet("translate")]
        public async Task<IActionResult> GetTranslation(string matter)
        {
            // Use the injected agent instead of 'new'
            Console.WriteLine(matter);
            var result = " ";
            try
            {
                 result = await _translationAgent.TranslateAsync(matter.Trim());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Ok(new { translatedText = result });
        }
    }
}