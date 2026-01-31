using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured");

using var httpClient = new HttpClient();

Console.WriteLine("=== .NET Prompt Generator ===");
Console.WriteLine("This tool will help optimize your problem statement into an effective ChatGPT prompt.");
Console.WriteLine();

var promptGenerator = new PromptGeneratorAgent(httpClient, apiKey);
await promptGenerator.RunAsync();
