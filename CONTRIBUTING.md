# Contributing to ModbusDirectConnect-CLI

Thank you for your interest in contributing to ModbusDirectConnect-CLI! This document provides guidelines for contributing to the project.

## Development Setup

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- Git
- A code editor (Visual Studio, VS Code, Rider, etc.)
- GitHub account (for accessing packages)

### Getting Started

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/ModbusDirectConnect-CLI.git
   cd ModbusDirectConnect-CLI
   ```

3. Set up GitHub Packages authentication (see [DEVELOPMENT.md](DEVELOPMENT.md) for details):
   ```bash
   dotnet nuget add source https://nuget.pkg.github.com/effektor/index.json \
     --name effektor \
     --username YOUR_GITHUB_USERNAME \
     --password YOUR_GITHUB_TOKEN \
     --store-password-in-clear-text
   ```

4. Create a new branch for your feature:
   ```bash
   git checkout -b feature/your-feature-name
   ```

5. Build the project:
   ```bash
   cd ModbusDirectConnect.CLI
   dotnet build
   ```

6. Make your changes and test them:
   ```bash
   dotnet run -- [your-test-commands]
   ```

## Code Style

- Follow C# coding conventions and best practices
- Use meaningful variable and method names
- Add XML documentation comments to public APIs
- Keep methods focused and concise
- Use async/await for I/O operations

## Testing

- Test your changes manually with various command combinations
- Ensure existing functionality is not broken
- Test on multiple platforms if possible (Windows, Linux, macOS)

## Submitting Changes

1. Commit your changes with clear, descriptive commit messages:
   ```bash
   git commit -m "Add feature: brief description"
   ```

2. Push to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```

3. Open a Pull Request on GitHub with:
   - A clear title
   - Description of what changes you made
   - Why the changes are needed
   - Any related issues

## Pull Request Guidelines

- Keep PRs focused on a single feature or bug fix
- Update documentation if needed
- Ensure the code builds successfully
- Follow the existing code structure and patterns

## Integrating with ModbusDirectConnect Library

The ModbusDirectConnect library (version 1.1.1) is now available on GitHub Packages.

### Setup Authentication

GitHub Packages requires authentication for all downloads, even from public repositories. Before you can work with the library:

1. Create a GitHub Personal Access Token (PAT) with `read:packages` scope
2. Configure NuGet authentication (see [DEVELOPMENT.md](DEVELOPMENT.md) for detailed instructions):
   ```bash
   dotnet nuget add source https://nuget.pkg.github.com/effektor/index.json \
     --name effektor \
     --username YOUR_GITHUB_USERNAME \
     --password YOUR_GITHUB_TOKEN \
     --store-password-in-clear-text
   ```

### Implementing the Integration

The package reference is already added in `ModbusDirectConnect.CLI.csproj`:
```xml
<PackageReference Include="ModbusDirectConnect" Version="1.1.1" />
```

To complete the integration in `ModbusTcpClient.cs`:

1. **Import the library namespace** at the top of the file
2. **Replace stub implementations** with actual library calls
3. **Update the connection logic** in `EnsureConnectedAsync()`
4. **Handle library-specific exceptions** appropriately
5. **Implement proper disposal** in the `Dispose()` method

Example integration pattern (adjust based on actual library API):
```csharp
// Add library instance
private ModbusDirectConnect.IModbusClient? _modbusClient;

// In EnsureConnectedAsync:
if (_modbusClient == null)
{
    _modbusClient = new ModbusDirectConnect.ModbusTcpClient(_host, _port);
    _modbusClient.SlaveId = _slaveId;
    _modbusClient.Timeout = TimeSpan.FromMilliseconds(_timeout);
    await _modbusClient.ConnectAsync();
}

// In ReadCoilsAsync:
return await _modbusClient.ReadCoilsAsync(_slaveId, startAddress, count);
```

### Adding RTU Support

Once TCP integration is complete, add RTU support:

1. Create `ModbusRtuClient.cs` implementing `IModbusClient`
2. Update `ModbusClientFactory.cs` to handle RTU protocol
3. Remove the `NotImplementedException` for RTU

### Testing

After integration:
1. Test against a real or simulated Modbus device
2. Verify all function codes work correctly
3. Test error handling and timeout scenarios

## Reporting Issues

- Use the GitHub issue tracker
- Check if the issue already exists
- Provide detailed information:
  - What you were trying to do
  - What happened
  - What you expected to happen
  - Your environment (OS, .NET version, etc.)
  - Steps to reproduce

## Feature Requests

- Open an issue with the "enhancement" label
- Describe the feature and its use case
- Explain why it would be valuable

## Questions

If you have questions, feel free to:
- Open a discussion on GitHub
- Create an issue with the "question" label

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.
