# ModbusDirectConnect PowerShell Module

PowerShell module for the ModbusDirectConnect CLI tool.

## Installation

### Manual Installation

1. Download the `ModbusDirectConnect-PowerShell.zip` from the releases
2. Extract to your PowerShell modules directory:
   - Windows PowerShell: `C:\Users\<YourName>\Documents\WindowsPowerShell\Modules\`
   - PowerShell Core: `C:\Users\<YourName>\Documents\PowerShell\Modules\`
3. Import the module:

```powershell
Import-Module ModbusDirectConnect
```

### Setting the CLI Path

If the `modbus-cli` executable is not in your PATH, you can set its location:

```powershell
Set-ModbusCliPath -Path "C:\path\to\modbus-cli.exe"
```

## Available Cmdlets

### Read Operations

- `Read-ModbusCoils` - Read coils (FC 1)
- `Read-ModbusDiscreteInputs` - Read discrete inputs (FC 2)
- `Read-ModbusHoldingRegisters` - Read holding registers (FC 3)
- `Read-ModbusInputRegisters` - Read input registers (FC 4)

### Write Operations

- `Write-ModbusCoil` - Write single coil (FC 5)
- `Write-ModbusRegister` - Write single register (FC 6)
- `Write-ModbusCoils` - Write multiple coils (FC 15)
- `Write-ModbusRegisters` - Write multiple registers (FC 16)

## Usage Examples

### Reading Data

```powershell
# Read 10 coils starting at address 0
Read-ModbusCoils -Address 0 -Count 10 -Host "192.168.1.100"

# Read 5 holding registers with custom timeout
Read-ModbusHoldingRegisters -Address 1000 -Count 5 -Host "192.168.1.100" -Timeout 10000

# Read discrete inputs from a different slave
Read-ModbusDiscreteInputs -Address 100 -Count 8 -Host "192.168.1.100" -SlaveId 2
```

### Writing Data

```powershell
# Write a single coil
Write-ModbusCoil -Address 0 -Value $true -Host "192.168.1.100"

# Write a single register
Write-ModbusRegister -Address 1000 -Value 12345 -Host "192.168.1.100"

# Write multiple coils
Write-ModbusCoils -Address 0 -Values @($true, $false, $true, $true) -Host "192.168.1.100"

# Write multiple registers
Write-ModbusRegisters -Address 1000 -Values @(100, 200, 300, 400) -Host "192.168.1.100"
```

### Advanced Usage

```powershell
# Use different protocol (when RTU support is available)
Read-ModbusHoldingRegisters -Address 1000 -Count 10 -Protocol "rtu" -Host "COM3"

# Different port
Read-ModbusHoldingRegisters -Address 1000 -Count 10 -Host "192.168.1.100" -Port 5502
```

## Getting Help

Use PowerShell's built-in help system:

```powershell
# List all commands
Get-Command -Module ModbusDirectConnect

# Get detailed help for a specific cmdlet
Get-Help Read-ModbusHoldingRegisters -Full

# Get examples
Get-Help Write-ModbusCoil -Examples
```

## Parameters

All cmdlets support these common parameters:

- `-Host` - Modbus host address (default: localhost)
- `-Port` - Modbus port (default: 502)
- `-SlaveId` - Modbus slave/unit ID (default: 1)
- `-Timeout` - Connection timeout in milliseconds (default: 5000)
- `-Protocol` - Protocol type: tcp or rtu (default: tcp)

## Requirements

- PowerShell 5.1 or later
- ModbusDirectConnect CLI executable (`modbus-cli` or `modbus-cli.exe`)

## License

See the main project [LICENSE](../LICENSE) for details.
