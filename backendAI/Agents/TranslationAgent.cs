using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace backendAI.Agents
{
    class TranslationAgent
    {
        private readonly ChatCompletionAgent _agent;

        public TranslationAgent(Kernel kernel)
        {
     
            _agent = new ChatCompletionAgent
            {
                Name = "ResearchTranslator",
                Instructions = """
                You are a professional academic translator specializing in scientific research papers. 
                Your goal is to translate the provided text into clear, academic English.
                
                Rules:
                1. Maintain all technical terminology and scientific notations.
                2. Preserve the original structure (headings, citations, and lists).
                3. If the text is already in English, return it exactly as it is.
                4. Do not add any conversational filler or introductory text. Just provide the translation.
                """,
                Kernel = kernel
            };
        }

        public async Task<string> TranslateAsync(string originalText)
        {
    
            string prompt = $"Translate the following research content to English:\n\n{originalText}";

            Console.WriteLine(prompt);
            string translation = string.Empty;

            await foreach (var message in _agent.InvokeAsync(prompt))
            {
                translation += message.Message.Content;
            }

            return translation;
        }
    }
}
