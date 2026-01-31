# User Secrets Manager - Instructions

## Setting up User Secrets for Development

User Secrets are securely stored outside your project directory, making them safe for local development without the risk of accidental commits.

### Windows PowerShell

```powershell
# Initialize user secrets (run once)
dotnet user-secrets init

# Set your OpenAI API key
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-actual-key-here"

# List all secrets to verify
dotnet user-secrets list

# Remove a secret
dotnet user-secrets remove "OpenAI:ApiKey"

# Clear all secrets
dotnet user-secrets clear
```

### Windows Command Prompt

```cmd
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-actual-key-here"
dotnet user-secrets list
```

### Linux/macOS

```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-actual-key-here"
dotnet user-secrets list
```

## Where Are User Secrets Stored?

- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>`
- **Linux/macOS**: `~/.microsoft/usersecrets/<user-secrets-id>`

The `<user-secrets-id>` is automatically generated and stored in your `.csproj` file.

## Obtaining Your OpenAI API Key

1. Go to https://platform.openai.com/api-keys
2. Sign in with your OpenAI account
3. Click "Create new secret key"
4. Copy the key (you won't see it again!)
5. Use the command above to set it

## Running the Application

Once configured, simply run:

```bash
dotnet run
```

The application will automatically load your secret configuration.
