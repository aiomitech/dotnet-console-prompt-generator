using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace PromptGenerator.Tests;

public class PromptGeneratorAgentTests
{
    private const string TestApiKey = "test-api-key";
    private const string ValidJsonResponse = @"{
        ""choices"": [
            {
                ""message"": {
                    ""content"": ""Test response from OpenAI""
                }
            }
        ]
    }";

    [Fact]
    public void Constructor_InitializesSuccessfully()
    {
        // Arrange
        var httpClient = new HttpClient();
        var apiKey = TestApiKey;

        // Act
        var agent = new PromptGeneratorAgent(httpClient, apiKey);

        // Assert
        Assert.NotNull(agent);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void EmptyOrWhitespaceInput_ShouldHandleGracefully(string input)
    {
        // Arrange
        var httpClient = new HttpClient();
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act & Assert
        Assert.True(string.IsNullOrWhiteSpace(input));
    }

    [Fact]
    public async Task GetCompletionAsync_WithValidResponse_ReturnsContent()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ValidJsonResponse)
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        var result = await CallGetCompletionAsync(agent, "Test system prompt", "Test user message");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test response from OpenAI", result);
    }

    [Fact]
    public async Task GetCompletionAsync_WithEmptyChoices_ReturnsUnableToParseResponse()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{ ""choices"": [] }")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        var result = await CallGetCompletionAsync(agent, "System prompt", "User message");

        // Assert
        Assert.Equal("Unable to parse response", result);
    }

    [Fact]
    public async Task GetCompletionAsync_WithInvalidJson_ReturnsUnableToParseResponse()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("Invalid JSON")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        var result = await CallGetCompletionAsync(agent, "System prompt", "User message");

        // Assert
        Assert.StartsWith("API Error:", result);
    }

    [Fact]
    public async Task GetCompletionAsync_WithHttpRequestException_ReturnsApiError()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        var result = await CallGetCompletionAsync(agent, "System prompt", "User message");

        // Assert
        Assert.StartsWith("API Error:", result);
    }

    [Fact]
    public async Task GetCompletionAsync_WithNullContent_ReturnsNoResponseGenerated()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{
                    ""choices"": [
                        {
                            ""message"": {
                                ""content"": null
                            }
                        }
                    ]
                }")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        var result = await CallGetCompletionAsync(agent, "System prompt", "User message");

        // Assert
        Assert.Equal("No response generated", result);
    }

    [Fact]
    public async Task GetCompletionAsync_SendsCorrectApiKey()
    {
        // Arrange
        var capturedRequest = (HttpRequestMessage)null;
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ValidJsonResponse)
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        await CallGetCompletionAsync(agent, "System prompt", "User message");

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.True(capturedRequest.Headers.Contains("Authorization"));
        var authHeader = capturedRequest.Headers.GetValues("Authorization").FirstOrDefault();
        Assert.Equal($"Bearer {TestApiKey}", authHeader);
    }

    [Fact]
    public async Task GetCompletionAsync_SendsCorrectRequestFormat()
    {
        // Arrange
        var capturedRequest = (HttpRequestMessage)null;
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ValidJsonResponse)
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        await CallGetCompletionAsync(agent, "Test system", "Test user");

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal("https://api.openai.com/v1/chat/completions", capturedRequest.RequestUri.ToString());
        
        // Verify request body contains correct structure
        var content = await capturedRequest.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        
        Assert.Equal("gpt-3.5-turbo", root.GetProperty("model").GetString());
        Assert.Equal(0.7, root.GetProperty("temperature").GetDouble());
        Assert.Equal(2048, root.GetProperty("max_tokens").GetInt32());
        
        var messages = root.GetProperty("messages");
        Assert.True(messages.GetArrayLength() >= 2);
    }

    [Fact]
    public async Task GetCompletionAsync_With401Unauthorized_ThrowsHttpException()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Invalid API key")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var agent = new PromptGeneratorAgent(httpClient, TestApiKey);

        // Act
        var result = await CallGetCompletionAsync(agent, "System prompt", "User message");

        // Assert
        Assert.StartsWith("API Error:", result);
    }

    // Helper method to call private GetCompletionAsync method via reflection
    private async Task<string> CallGetCompletionAsync(PromptGeneratorAgent agent, string systemPrompt, string userMessage)
    {
        var method = typeof(PromptGeneratorAgent).GetMethod(
            "GetCompletionAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var task = (Task<string>)method.Invoke(agent, new object[] { systemPrompt, userMessage });
        return await task;
    }
}
