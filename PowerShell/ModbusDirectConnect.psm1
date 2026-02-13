# ModbusDirectConnect PowerShell Module
# This module provides PowerShell cmdlets for the ModbusDirectConnect CLI

# Get the path to the mbdc executable
$script:ModbusCliPath = $null

function Get-ModbusCliPath {
    if ($null -ne $script:ModbusCliPath -and (Test-Path $script:ModbusCliPath)) {
        return $script:ModbusCliPath
    }
    
    # Try to find mbdc in PATH
    $cliCommand = Get-Command mbdc -ErrorAction SilentlyContinue
    if ($cliCommand) {
        $script:ModbusCliPath = $cliCommand.Source
        return $script:ModbusCliPath
    }
    
    # Try to find in module directory
    $moduleDir = Split-Path -Parent $PSScriptRoot
    $possiblePaths = @(
        (Join-Path $moduleDir "ModbusDirectConnect.CLI\bin\Release\net8.0\mbdc.exe"),
        (Join-Path $moduleDir "ModbusDirectConnect.CLI\bin\Debug\net8.0\mbdc.exe"),
        (Join-Path $moduleDir "mbdc.exe"),
        (Join-Path $moduleDir "mbdc")
    )
    
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $script:ModbusCliPath = $path
            return $script:ModbusCliPath
        }
    }
    
    throw "mbdc executable not found. Please ensure it is installed and in your PATH, or set the path using Set-ModbusCliPath."
}

function Set-ModbusCliPath {
    <#
    .SYNOPSIS
        Sets the path to the mbdc executable
    .PARAMETER Path
        Full path to the mbdc executable
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path
    )
    
    if (-not (Test-Path $Path)) {
        throw "The specified path does not exist: $Path"
    }
    
    $script:ModbusCliPath = $Path
}

function Invoke-ModbusCli {
    param(
        [string[]]$Arguments
    )
    
    $cliPath = Get-ModbusCliPath
    
    # On Windows, use .exe; on Unix, use the dll with dotnet
    if ($IsWindows -or $PSVersionTable.PSVersion.Major -le 5) {
        & $cliPath @Arguments
    } else {
        # For cross-platform, try using dotnet
        $dllPath = $cliPath -replace '\.exe$', '.dll'
        if (Test-Path $dllPath) {
            & dotnet $dllPath @Arguments
        } else {
            & $cliPath @Arguments
        }
    }
}

function Get-ProtocolArgs {
    param(
        [string]$Protocol
    )

    if ($Protocol -eq "rtu") {
        return @("--rtu")
    }

    return @()
}

function Convert-TimeoutMsToSeconds {
    param(
        [int]$Milliseconds
    )

    if ($Milliseconds -lt 0) {
        throw "Timeout must be zero or greater."
    }

    return ([double]$Milliseconds / 1000).ToString("0.###", [System.Globalization.CultureInfo]::InvariantCulture)
}

function Get-ModbusCoil {
    <#
    .SYNOPSIS
        Reads coils from a Modbus device
    .PARAMETER Address
        Starting address to read from
    .PARAMETER Count
        Number of coils to read
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [uint16]$Count,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $args = @(
        $Host,
        "--read-coil", $Address,
        "--count", $Count,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

function Get-ModbusDiscreteInput {
    <#
    .SYNOPSIS
        Reads discrete inputs from a Modbus device
    .PARAMETER Address
        Starting address to read from
    .PARAMETER Count
        Number of discrete inputs to read
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [uint16]$Count,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $args = @(
        $Host,
        "--read-discrete", $Address,
        "--count", $Count,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

function Get-ModbusHoldingRegister {
    <#
    .SYNOPSIS
        Reads holding registers from a Modbus device
    .PARAMETER Address
        Starting address to read from
    .PARAMETER Count
        Number of holding registers to read
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [uint16]$Count,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $args = @(
        $Host,
        "--read-holding", $Address,
        "--count", $Count,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

function Get-ModbusInputRegister {
    <#
    .SYNOPSIS
        Reads input registers from a Modbus device
    .PARAMETER Address
        Starting address to read from
    .PARAMETER Count
        Number of input registers to read
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [uint16]$Count,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $args = @(
        $Host,
        "--read-inputreg", $Address,
        "--count", $Count,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

function Set-ModbusCoil {
    <#
    .SYNOPSIS
        Writes a single coil to a Modbus device
    .PARAMETER Address
        Address to write to
    .PARAMETER Value
        Boolean value to write
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [bool]$Value,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $dataValue = if ($Value) { "1" } else { "0" }
    $args = @(
        $Host,
        "--write-coil", $Address,
        "--data", $dataValue,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

function Set-ModbusRegister {
    <#
    .SYNOPSIS
        Writes a single holding register to a Modbus device
    .PARAMETER Address
        Address to write to
    .PARAMETER Value
        Register value to write (0-65535)
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [uint16]$Value,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $args = @(
        $Host,
        "--write-reg", $Address,
        "--data", $Value,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

function Set-ModbusCoils {
    <#
    .SYNOPSIS
        Writes multiple coils to a Modbus device
    .PARAMETER Address
        Starting address to write to
    .PARAMETER Values
        Array of boolean values to write
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [bool[]]$Values,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $valuesStr = ($Values | ForEach-Object { if ($_ ) { "1" } else { "0" } }) -join ","
    
    $args = @(
        $Host,
        "--write-multi-coil", $Address,
        "--data", $valuesStr,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

function Set-ModbusRegisters {
    <#
    .SYNOPSIS
        Writes multiple holding registers to a Modbus device
    .PARAMETER Address
        Starting address to write to
    .PARAMETER Values
        Array of register values to write (0-65535)
    .PARAMETER Host
        Modbus host address (default: localhost)
    .PARAMETER Port
        Modbus port (default: 502)
    .PARAMETER SlaveId
        Modbus slave/unit ID (default: 1)
    .PARAMETER Timeout
        Connection timeout in milliseconds (default: 5000)
    .PARAMETER Protocol
        Protocol type: tcp or rtu (default: tcp)
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [uint16]$Address,
        
        [Parameter(Mandatory=$true)]
        [uint16[]]$Values,
        
        [string]$Host = "localhost",
        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$Timeout = 5000,
        [ValidateSet("tcp", "rtu")]
        [string]$Protocol = "tcp"
    )
    
    $valuesStr = $Values -join ","
    
    $args = @(
        $Host,
        "--write-multi-reg", $Address,
        "--data", $valuesStr,
        "--port", $Port,
        "--slave", $SlaveId,
        "--timeout", (Convert-TimeoutMsToSeconds -Milliseconds $Timeout)
    ) + (Get-ProtocolArgs -Protocol $Protocol)
    
    Invoke-ModbusCli -Arguments $args
}

# Export module members
Export-ModuleMember -Function @(
    'Get-ModbusCliPath',
    'Set-ModbusCliPath',
    'Get-ModbusCoil',
    'Get-ModbusDiscreteInput',
    'Get-ModbusHoldingRegister',
    'Get-ModbusInputRegister',
    'Set-ModbusCoil',
    'Set-ModbusRegister',
    'Set-ModbusCoils',
    'Set-ModbusRegisters'
)
