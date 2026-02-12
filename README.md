# ModbusDirectConnect-CLI

A cross-platform command-line tool for reading and writing Modbus addresses using TCP or RTU protocols.

This CLI tool is a wrapper over the [github.com/Effektor/ModbusDirectConnect](https://github.com/Effektor/ModbusDirectConnect) library, providing easy-to-use commands for Modbus communication.

## Features

- **Cross-platform**: Works on Linux, Windows, and macOS
- **Multiple protocols**: Supports both Modbus TCP and RTU
- **Full Modbus function code support**:
  - Read Coils (FC 1)
  - Read Discrete Inputs (FC 2)
  - Read Holding Registers (FC 3)
  - Read Input Registers (FC 4)
  - Write Single Coil (FC 5)
  - Write Single Register (FC 6)
  - Write Multiple Coils (FC 15)
  - Write Multiple Registers (FC 16)
- **PowerShell integration**: Optional PowerShell module for Windows users
- **Linux/Bash conventions**: Command-line syntax follows standard Unix conventions

## Installation

### From Release Binaries

Download the latest release for your platform from the [Releases page](https://github.com/Effektor/ModbusDirectConnect-CLI/releases):

- **Linux**: `modbus-cli-linux-x64.tar.gz`
- **Windows**: `modbus-cli-windows-x64.zip`
- **macOS**: `modbus-cli-macos-x64.tar.gz`

Extract the archive and add the executable to your PATH.

### From Source

Requirements:
- .NET 8.0 SDK or later

```bash
git clone https://github.com/Effektor/ModbusDirectConnect-CLI.git
cd ModbusDirectConnect-CLI/ModbusDirectConnect.CLI
dotnet build -c Release
```

The built executable will be in `bin/Release/net8.0/`.

## Usage

### Basic Syntax

```bash
modbus-cli [command] [arguments] [options]
```

### Global Options

- `-H, --host <host>`: Modbus host address (default: localhost)
- `-p, --port <port>`: Modbus port (default: 502)
- `-s, --slave <id>`: Modbus slave/unit ID (default: 1)
- `-t, --timeout <ms>`: Connection timeout in milliseconds (default: 5000)
- `--protocol <type>`: Protocol type - tcp or rtu (default: tcp)

### Read Commands

#### Read Coils

```bash
modbus-cli read coils <address> <count> [options]
```

Example:
```bash
modbus-cli read coils 0 10 --host 192.168.1.100 --slave 1
```

#### Read Discrete Inputs

```bash
modbus-cli read discrete-inputs <address> <count> [options]
```

Example:
```bash
modbus-cli read discrete-inputs 100 5 --host 192.168.1.100
```

#### Read Holding Registers

```bash
modbus-cli read holding-registers <address> <count> [options]
```

Example:
```bash
modbus-cli read holding-registers 1000 10 --host 192.168.1.100 --port 502
```

#### Read Input Registers

```bash
modbus-cli read input-registers <address> <count> [options]
```

Example:
```bash
modbus-cli read input-registers 2000 5 --host 192.168.1.100
```

### Write Commands

#### Write Single Coil

```bash
modbus-cli write coil <address> <value> [options]
```

Example:
```bash
modbus-cli write coil 0 true --host 192.168.1.100
modbus-cli write coil 1 false --host 192.168.1.100
```

#### Write Single Register

```bash
modbus-cli write register <address> <value> [options]
```

Example:
```bash
modbus-cli write register 1000 12345 --host 192.168.1.100
```

#### Write Multiple Coils

```bash
modbus-cli write coils <address> <values> [options]
```

Values should be comma-separated (true/false or 1/0).

Example:
```bash
modbus-cli write coils 0 "true,false,true,true" --host 192.168.1.100
modbus-cli write coils 0 "1,0,1,1" --host 192.168.1.100
```

#### Write Multiple Registers

```bash
modbus-cli write registers <address> <values> [options]
```

Values should be comma-separated numbers.

Example:
```bash
modbus-cli write registers 1000 "100,200,300,400" --host 192.168.1.100
```

## PowerShell Module

For Windows PowerShell users, a dedicated PowerShell module is available that provides native PowerShell cmdlets.

### Installation

1. Download `ModbusDirectConnect-PowerShell.zip` from the [Releases page](https://github.com/Effektor/ModbusDirectConnect-CLI/releases)
2. Extract to a PowerShell modules directory (e.g., `C:\Users\<YourName>\Documents\PowerShell\Modules\`)
3. Import the module:

```powershell
Import-Module ModbusDirectConnect
```

### PowerShell Usage

The module provides the following cmdlets:

- `Read-ModbusCoils`
- `Read-ModbusDiscreteInputs`
- `Read-ModbusHoldingRegisters`
- `Read-ModbusInputRegisters`
- `Write-ModbusCoil`
- `Write-ModbusRegister`
- `Write-ModbusCoils`
- `Write-ModbusRegisters`

Example:
```powershell
# Read 10 holding registers starting at address 1000
Read-ModbusHoldingRegisters -Address 1000 -Count 10 -Host "192.168.1.100"

# Write a single coil
Write-ModbusCoil -Address 0 -Value $true -Host "192.168.1.100"

# Write multiple registers
Write-ModbusRegisters -Address 1000 -Values @(100, 200, 300) -Host "192.168.1.100"
```

Use `Get-Help <cmdlet-name>` for detailed help on each cmdlet.

## CI/CD Pipeline

The project includes a GitHub Actions workflow that:

- Builds the CLI for Linux, Windows, and macOS
- Packages the PowerShell module
- Runs tests
- Creates release artifacts
- Publishes releases automatically on version tags

To create a release:

1. Tag the commit with a version (e.g., `v1.0.0`)
2. Push the tag to GitHub
3. The CI pipeline will automatically build and create a release

```bash
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

## Development

### Building

```bash
cd ModbusDirectConnect.CLI
dotnet build
```

### Running

```bash
cd ModbusDirectConnect.CLI
dotnet run -- [arguments]
```

### Testing

```bash
cd ModbusDirectConnect.CLI
dotnet test
```

### Publishing

To create a self-contained executable:

```bash
# Linux
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

# Windows
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# macOS
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
```

## Architecture

The CLI is structured with clean separation of concerns:

```
ModbusDirectConnect.CLI/
├── Program.cs              # Entry point and command setup
├── Commands/               # Command implementations
│   ├── ReadCommands.cs     # Read operations
│   └── WriteCommands.cs    # Write operations
└── Client/                 # Modbus client abstraction
    ├── IModbusClient.cs    # Client interface
    ├── ModbusClientFactory.cs
    └── ModbusTcpClient.cs  # TCP implementation
```

The client layer provides an abstraction over the ModbusDirectConnect library, making it easy to:
- Add new protocols (RTU, etc.)
- Swap implementations
- Mock for testing

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

See the [LICENSE](LICENSE) file for details.

## Related Projects

- [ModbusDirectConnect](https://github.com/Effektor/ModbusDirectConnect) - The underlying C# Modbus library
