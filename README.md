# .NET Console Prompt Generator

A sophisticated .NET console application that uses AI to transform your problem statements into optimized, production-ready prompts for ChatGPT. The tool guides your problems through a multi-stage pipeline using the OpenAI API's Chat Completions endpoint.

## Features

- **Problem Analysis**: Automatically analyzes your problem to identify the core issue, domain, complexity, and missing information
- **Context Enrichment**: Provides relevant background information, assumptions to clarify, and best practices
- **Prompt Optimization**: Generates a polished, specific prompt ready to copy-paste into ChatGPT
- **OpenAI Integration**: Direct integration with OpenAI's Chat Completions API (gpt-3.5-turbo model)
- **User-Friendly Interface**: Simple console-based interaction

## Architecture

The application implements a multi-stage prompt generation pipeline:

1. **Analysis Stage**: Examines the user's problem to extract key characteristics
2. **Enrichment Stage**: Gathers context and best practices using the analysis
3. **Optimization Stage**: Generates a final, polished prompt using all previous insights

## Prerequisites

- .NET 8.0 or later
- An OpenAI API key (from https://platform.openai.com/api-keys)

## Setup

### 1. Navigate to the Project
```bash
cd src/PromptGenerator
```

### 2. Configure OpenAI API Key

This project uses User Secrets for secure API key management (recommended for local development).

**Using User Secrets (Recommended)**:
```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "your-openai-api-key-here"
```

**Or using Environment Variable**:
```bash
set OPENAI_API_KEY=your-openai-api-key-here
```

## Building and Running

### Build the Project
From the `src/PromptGenerator` directory:
```bash
dotnet build
```

### Run the Application
From the `src/PromptGenerator` directory:
```bash
dotnet run
```

### Example Session
```
=== .NET Prompt Generator ===
This tool will help optimize your problem statement into an effective ChatGPT prompt.

Enter your problem or question: How can I improve the performance of my database queries?

Processing your problem through the prompt optimization pipeline...

Analysis:
- Core problem: Database query performance optimization
- Domain: Technical (Database/Software Engineering)
- Complexity level: Intermediate to Advanced
...

Context Enrichment:
- Relevant best practices: Query indexing, execution plans, query optimization
- Common pitfalls: Over-indexing, N+1 queries, poor join strategies
...

=== OPTIMIZED PROMPT FOR ChatGPT ===
[Complete optimized prompt ready to copy-paste]
```

## Project Structure
dotnet-console-prompt_generator/
├── src/
│   └── PromptGenerator/
│       ├── PromptGenerator.csproj     # Project file with dependencies
│       ├── Program.cs                 # Entry point and OpenAI configuration
│       └── PromptGeneratorAgent.cs    # Core agent logic with 3-stage pipeline
├── docs/
│   ├── SETUP.md                       # Detailed setup instructions
│   └── CHANGELOG.md                   # Version history and features
├── System.Net.Http.Json** (v5.0.0): HTTP JSON serialization utilities
└── dotnet-console-prompt_generator.sln # Solutiony point
├── PromptGeneratorAgent.cs          # Core agent logic
└── README.md                        # This file
```

## Dependencies

- **System.Net.Http.Json** (v5.0.0): HTTP client utilities for JSON serialization
- **Microsoft.Extensions.Configuration** (v8.0.0): Configuration management
- **Microsoft.Extensions.Configuration.UserSecrets** (v8.0.0): Secure local credential management via User Secrets

## How It Works

### System Prompts

The agent uses three specialized system prompts:

1. **Analysis Prompt**: Identifies problem characteristics
   - Core problem extraction
   - Domain classification
   - Complexity assessment
   - Missing information detection

2. **Context Prompt**: Enriches the problem understanding
   - Background information
   - Assumption clarification
   - Domain-specific best practices
   - Common pitfalls

3. **Optimization Prompt**: Generates the final prompt
   - Creates clear, specific prompts
   - Includes context integration
   - Specifies output formats
   - Provides examples and constraints

## Security Notes

- **Never** commit your OpenAI API key to version control
- Use User Secrets application in `src/PromptGenerator/` to:

- **System Prompts**: Edit the prompt strings in `PromptGeneratorAgent.cs` to:
  - Adjust the analysis focus
  - Change the enrichment strategy
  - Customize the output format
  - Tailor prompts for specific domains

- **API Model**: Change the model in `PromptGeneratorAgent.cs` from `gpt-3.5-turbo` to `gpt-4`, `gpt-4-turbo`, etc.

- **Token Limits**: Adjust `max_tokens` values in the API requests for each stage

You can modify the system prompts in `PromptGeneratorAgent.cs` to:
- Adjust the analysis focus
- Change the enrichment strategy
- Customize the output format
- Tailor prompts for specific domains

## Example Use Cases

- **Writing Help**: Optimize requests for creative writing assistance
- **Code Issues**: Generate detailed prompts for debugging help
- **Research**: Create comprehensive prompts for research topics
- **Learning**: Craft effective teaching and tutoring prompts
- **Technical Documentation**: Generate prompts for documentation help

## Troubleshooting

### "OpenAI API key not configured"
- Ensure you've set up User Secrets or environment variables
- Run `dotnet user-secrets list` to verify the key is stored

### API Errors
- Check that your OpenAI API key is valid
- Verify you have API credits in your OpenAI account
- Ensure your internet connection is active

### Build Errors
- Ensure you have .NET 8.0 or later: `dotnet --version`
- Run `dotnet restore` to restore packages

## Future Enhancements

- Add prompt history and templates
- Support for multiple AI providers
- Interactive prompt refinement loop
- Export prompts to file or clipboard
- Batch processing for multiple problems
- Custom domain-specific templates
- Conversation history tracking

## License

MIT

## Support

For issues or questions, please open an issue in the repository or contact the maintainers.
