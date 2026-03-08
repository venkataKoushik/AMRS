
using backendAI.Agents;
using backendAI.Plugins;
using Microsoft.SemanticKernel;

namespace backendAI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);




            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // reading from appsettings.json fiile
            IConfiguration config = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                    .Build();
            var modelId = config["AzureOpenAi:ModelId"] ?? "gpt4.1-mini";
            var endpoint = config["AzureOpenAi:EndPoint"] ?? "";
            var apiKey = config["AzureOpenAi:ApiKey"] ?? "";

            // adding kernel to service
            builder.Services.AddTransient<Kernel>(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

                return kernelBuilder.Build();
            });


            

            builder.Services.AddTransient<TranslationAgent>();
            builder.Services.AddTransient<IngestionAgent>();
                builder.Services.AddTransient<PreProcessAgent>();


            builder.Services.AddSingleton<OCRPlugin>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
