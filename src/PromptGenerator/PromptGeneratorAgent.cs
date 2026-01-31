using System.Text;
using System.Text.Json;

public class PromptGeneratorAgent
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string OpenAiUrl = "https://api.openai.com/v1/chat/completions";

    public PromptGeneratorAgent(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task RunAsync()
    {
        Console.Write("Enter your problem or question: ");
        var userProblem = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userProblem))
        {
            Console.WriteLine("No problem provided. Exiting.");
            return;
        }

        Console.WriteLine("\nProcessing your problem through the prompt optimization pipeline...\n");

        try
        {
            // Step 1: Analyze the problem
            var analysis = await AnalyzeProblemAsync(userProblem);
            Console.WriteLine($"Analysis:\n{analysis}\n");

            // Step 2: Generate context enrichment
            var context = await EnrichContextAsync(userProblem, analysis);
            Console.WriteLine($"Context Enrichment:\n{context}\n");

            // Step 3: Generate optimized prompt
            var optimizedPrompt = await GenerateOptimizedPromptAsync(userProblem, analysis, context);

            Console.WriteLine("=== OPTIMIZED PROMPT FOR ChatGPT ===");
            Console.WriteLine(optimizedPrompt);
            Console.WriteLine("\n(Press Enter to exit)");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task<string> AnalyzeProblemAsync(string problem)
    {
        var systemPrompt = @"You are an expert prompt engineer. Your task is to analyze user problems and identify:
1. The core problem or question
2. The domain/category (technical, creative, analytical, etc.)
3. The complexity level
4. Any missing information that might be needed
5. The likely intent or goal

Be concise and bullet-pointed in your response.";

        return await GetCompletionAsync(systemPrompt, $"Analyze this problem: {problem}");
    }

    private async Task<string> EnrichContextAsync(string problem, string analysis)
    {
        var systemPrompt = @"You are an expert at gathering context. Given a problem and its analysis, provide:
1. Relevant background information that would be helpful
2. Assumptions to clarify
3. Best practices in the relevant domain
4. Common pitfalls to avoid

Be practical and actionable.";

        return await GetCompletionAsync(systemPrompt, $@"Problem: {problem}

Analysis: {analysis}");
    }

    private async Task<string> GenerateOptimizedPromptAsync(string problem, string analysis, string context)
    {
        var systemPrompt = @"You are an expert prompt engineer specializing in creating clear, effective prompts for AI systems like ChatGPT.

Your task is to transform a user's problem statement into a highly effective prompt that will:
1. Be specific and unambiguous
2. Include relevant context
3. Specify the desired output format
4. Provide examples when helpful
5. Include any constraints or requirements
6. Be actionable and ready to copy-paste

Create a prompt that's professional and comprehensive.";

        return await GetCompletionAsync(systemPrompt, $@"Original Problem: {problem}

Analysis: {analysis}

Context: {context}

Please generate an optimized, production-ready prompt that I can copy and paste into ChatGPT.");
    }

    private async Task<string> GetCompletionAsync(string systemPrompt, string userMessage)
    {
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            temperature = 0.7,
            max_tokens = 2048
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(OpenAiUrl, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out var message) && 
                    message.TryGetProperty("content", out var result))
                {
                    return result.GetString() ?? "No response generated";
                }
            }

            return "Unable to parse response";
        }
        catch (HttpRequestException ex)
        {
            return $"API Error: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"API Error: {ex.Message}";
        }
    }
}
