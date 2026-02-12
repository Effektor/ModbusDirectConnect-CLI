# Development Setup Guide

This guide is for **contributors and developers** who want to build the project from source or contribute code.

**End users:** Just download the pre-built binaries from the [Releases page](https://github.com/Effektor/ModbusDirectConnect-CLI/releases) - no setup required!

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- Git
- A code editor (optional, but recommended)

## GitHub Packages Authentication

The ModbusDirectConnect library dependency is hosted on GitHub Packages. Due to GitHub's platform requirements, authentication is needed to download packages during build, even though this is a public repository.

**This only affects building from source** - end users don't need any of this.

### Quick Setup

1. Create a GitHub Personal Access Token:
   - Go to [GitHub Settings → Personal Access Tokens](https://github.com/settings/tokens)
   - Create a token with `read:packages` scope
   - Copy the token

2. Configure NuGet:
   ```bash
   dotnet nuget add source https://nuget.pkg.github.com/effektor/index.json \
     --name effektor \
     --username YOUR_GITHUB_USERNAME \
     --password YOUR_GITHUB_TOKEN \
     --store-password-in-clear-text
   ```

3. Build the project:
   ```bash
   cd ModbusDirectConnect.CLI
   dotnet restore
   dotnet build
   ```

## Building the Project

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

### Error: 401 Unauthorized

Your authentication isn't configured. Follow the Quick Setup steps above.

### Package Not Found

Make sure you're using the correct package source URL and that your token is valid.

## Publishing Binaries

The CI/CD pipeline automatically handles authentication and builds binaries for all platforms. See `.github/workflows/build-and-release.yml` for details.

## Security

- Never commit tokens to source control
- The `nuget.config` file doesn't contain credentials
- Tokens with `read:packages` scope are sufficient for development

## Additional Resources

- [GitHub Packages Documentation](https://docs.github.com/en/packages)
- [.NET CLI Reference](https://learn.microsoft.com/en-us/dotnet/core/tools/)


