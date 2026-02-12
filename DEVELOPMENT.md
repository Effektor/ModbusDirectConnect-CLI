# Development Setup Guide

This guide explains how to set up your development environment for ModbusDirectConnect-CLI.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- Git
- GitHub account (required for accessing packages from GitHub Packages)

## About GitHub Packages Authentication

The ModbusDirectConnect library is distributed via GitHub Packages. **GitHub Packages requires authentication for all package downloads, even from public repositories.** This is a GitHub platform limitation, not specific to this project.

For end users, we recommend downloading pre-built binaries from the [Releases page](https://github.com/Effektor/ModbusDirectConnect-CLI/releases) instead of building from source.

## Setting up GitHub Packages Authentication

If you need to build from source or contribute to the project, follow these steps:

### Creating a Personal Access Token (PAT)

1. Go to GitHub Settings → Developer settings → [Personal access tokens → Tokens (classic)](https://github.com/settings/tokens)
2. Click "Generate new token (classic)"
3. Give it a descriptive name (e.g., "NuGet Package Access")
4. Select the following scope:
   - `read:packages` - Required to download packages from GitHub Packages
5. Click "Generate token" and **copy the token** (you won't be able to see it again)

### Configuring NuGet Authentication

#### Option 1: Using dotnet CLI (Recommended)

```bash
dotnet nuget add source https://nuget.pkg.github.com/effektor/index.json \
  --name effektor \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text
```

Replace:
- `YOUR_GITHUB_USERNAME` with your GitHub username
- `YOUR_GITHUB_TOKEN` with the Personal Access Token you created

#### Option 2: Using Environment Variable

Set the `NUGET_AUTH_TOKEN` environment variable:

**Bash/Zsh:**
```bash
export NUGET_AUTH_TOKEN=YOUR_GITHUB_TOKEN
```

**PowerShell:**
```powershell
$env:NUGET_AUTH_TOKEN="YOUR_GITHUB_TOKEN"
```

**Windows Command Prompt:**
```cmd
set NUGET_AUTH_TOKEN=YOUR_GITHUB_TOKEN
```

Then configure the source:
```bash
dotnet nuget add source https://nuget.pkg.github.com/effektor/index.json \
  --name effektor \
  --username YOUR_GITHUB_USERNAME \
  --password %NUGET_AUTH_TOKEN%
```

### Verifying Authentication

Test that you can restore packages:

```bash
cd ModbusDirectConnect.CLI
dotnet restore
```

If successful, you should see:
```
Restored /path/to/ModbusDirectConnect.CLI/ModbusDirectConnect.CLI.csproj
```

## Building the Project

Once authentication is set up:

```bash
cd ModbusDirectConnect.CLI
dotnet build
```

## Running the CLI

```bash
cd ModbusDirectConnect.CLI
dotnet run -- [command] [arguments]
```

Example:
```bash
dotnet run -- read holding-registers 1000 10 --host localhost
```

## Troubleshooting

### Error: NU1301 - Failed to retrieve information

This means authentication is not configured. Make sure:
1. Your PAT has the `read:packages` scope
2. You've configured the credentials using one of the methods above
3. You're using a valid GitHub account

### Error: 401 Unauthorized

Your token may have expired or doesn't have the correct permissions. Generate a new PAT and update your configuration.

### Package Not Found

Ensure:
- You're using the correct package source URL
- The ModbusDirectConnect package version (1.1.1) exists in the registry
- Your token has not expired

## CI/CD Authentication

The GitHub Actions workflow automatically handles authentication using the `GITHUB_TOKEN` secret, which is available in all GitHub Actions workflows. No additional configuration is needed for CI/CD.

## Security Best Practices

- **Never commit your PAT to source control**
- **Never share your PAT publicly**
- Add `.env` files or any files containing tokens to `.gitignore`
- Consider using a token with minimal required scopes
- Rotate tokens periodically
- Revoke tokens you're no longer using

The `nuget.config` file in this repository does not contain credentials and is safe to commit.

## Why Pre-built Binaries are Recommended

To make it easier for end users who just want to use the tool:
1. Pre-built binaries don't require .NET SDK installation
2. No authentication setup is needed
3. Single executable file - just download and run
4. Faster to get started

Building from source is primarily for:
- Contributors developing new features
- Users who want to modify the code
- Users who want to verify the source code

## Additional Resources

- [GitHub Packages Documentation](https://docs.github.com/en/packages)
- [Authenticating to GitHub Packages](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-to-github-packages)
- [NuGet Configuration](https://learn.microsoft.com/en-us/nuget/consume-packages/consuming-packages-authenticated-feeds)
- [.NET CLI Reference](https://learn.microsoft.com/en-us/dotnet/core/tools/)

