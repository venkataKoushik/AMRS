using backendAI.Models;
using backendAI.Plugins;
using Microsoft.SemanticKernel;
using OpenAI.RealtimeConversation;
using System.Threading.Tasks;

namespace backendAI.Agents
{
    public class PreProcessAgent
    {
        private string tessdataPath = "tessdata";
        private OCRPlugin _ocrPlugin;
        private readonly Kernel _kernel;

        public PreProcessAgent(OCRPlugin ocrPlugin , Kernel kernel)

        {
            this._ocrPlugin= ocrPlugin;
            this._kernel = kernel;
        }
        public async Task<PreProcessResult> Process(string filePath)
        {
            var result = new PreProcessResult();

            try
            {
                // Step 1: Validate file type
                string ext = Path.GetExtension(filePath).ToLower();

                if (ext != ".pdf" && ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext!=".docx" && ext != ".doc" && ext!=".txt")
                {
                    result.Success = false;
                    result.Error = "Unsupported file type";
                    return result;
                }



                // Step 2: Read file bytes
                byte[] fileBytes = File.ReadAllBytes(filePath);


                string fileName = Path.GetFileName(filePath);
                string text = "";
                //extracta text

                if (ext == ".pdf")
                   
                 {
                    text = _ocrPlugin.ExtractFromPdf(fileBytes);
                }
                else if ( ext == ".doc"   || ext == ".txt")
                {
                    text = System.Text.Encoding.UTF8.GetString(fileBytes);
                }
                else if(ext == ".docx")
                {
                    text=_ocrPlugin.ExtractFromDocx(fileBytes);
                }
                else
                {
                    text = _ocrPlugin.ExtractFromImage(fileBytes);
                }

                // Step 4: Detect language
                string language = await  DetectLanguageAsync(text);

               

                result.Success = true;
                result.Text = text;
                result.Language = language;

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                return result;
            }
        }



        public async Task<string> DetectLanguageAsync(string text)
        {
            var prompt = """
        Detect the language of the following text.
        Return only the language name.

        Text:
        {{$input}}
        """;

            var function = _kernel.CreateFunctionFromPrompt(prompt);

            var result = await _kernel.InvokeAsync(function, new()
            {
                ["input"] = text
            });

            return result.ToString().Trim();
        }
    }
}
