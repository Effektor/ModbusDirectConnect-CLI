# ModbusDirectConnect PowerShell Module
# Lightweight cmdlets backed by the ModbusDirectConnect .NET runtime.

$script:ModbusRuntimePath = $null
$script:ModbusRuntimeLoaded = $false

function Get-ModbusRuntimePath {
    if ($null -ne $script:ModbusRuntimePath -and (Test-Path $script:ModbusRuntimePath)) {
        return $script:ModbusRuntimePath
    }

    $repoRoot = Split-Path -Parent $PSScriptRoot
    $candidates = @(
        (Join-Path $PSScriptRoot 'lib/mbdc.dll'),
        (Join-Path $repoRoot 'ModbusDirectConnect.CLI/bin/Release/net10.0/mbdc.dll'),
        (Join-Path $repoRoot 'ModbusDirectConnect.CLI/bin/Debug/net10.0/mbdc.dll')
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            $script:ModbusRuntimePath = $candidate
            return $script:ModbusRuntimePath
        }
    }

    throw "Modbus runtime assembly not found. Expected mbdc.dll in '$PSScriptRoot/lib' for packaged modules."
}

function Initialize-ModbusRuntime {
    if ($script:ModbusRuntimeLoaded) {
        return
    }

    $runtimePath = Get-ModbusRuntimePath
    Add-Type -Path $runtimePath
    $script:ModbusRuntimeLoaded = $true
}

function Test-SerialTarget {
    param(
        [string]$Target
    )

    if ([string]::IsNullOrWhiteSpace($Target)) {
        return $false
    }

    return $Target -match '^(?:(?:\\\\\.\\)?COM\d+|/dev/.+)$'
}

function Resolve-ModbusTransport {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Transport,

        [string]$Target
    )

    $normalized = $Transport.Trim().ToLowerInvariant()

    switch ($normalized) {
        'auto' {
            if (Test-SerialTarget -Target $Target) {
                return 'rtu-serial'
            }

            return 'tcp'
        }
        'tcp' { return 'tcp' }
        'tcp/ip' { return 'tcp' }
        'rtu-tcp' { return 'rtu-tcp' }
        'tcp/ip-rtu' { return 'rtu-tcp' }
        'rtu-serial' { return 'rtu-serial' }
        'serial' { return 'rtu-serial' }
        default {
            throw "Unsupported transport '$Transport'."
        }
    }
}

function New-ModbusSession {
    param(
        [string]$Target,
        [int]$Port,
        [byte]$SlaveId,
        [int]$TimeoutMs,
        [string]$Transport,
        [int]$Baud
    )

    Initialize-ModbusRuntime

    if ($TimeoutMs -lt 0) {
        throw 'TimeoutMs must be zero or greater.'
    }

    $resolvedTransport = Resolve-ModbusTransport -Transport $Transport -Target $Target

    $serialBaud = $null
    if ($resolvedTransport -eq 'rtu-serial') {
        if ($Baud -le 0) {
            throw 'Baud must be a positive integer.'
        }

        $serialBaud = $Baud
    }

    $options = [ModbusDirectConnect.CLI.Transport.ConnectionOptions]::new(
        $Target,
        $null,
        $Port,
        $SlaveId,
        $TimeoutMs,
        0,
        $resolvedTransport,
        $null,
        $serialBaud,
        8,
        'N',
        '1'
    )

    $connection = [ModbusDirectConnect.CLI.Transport.EndpointResolver]::Resolve($options)
    $client = [ModbusDirectConnect.CLI.Client.ModbusClientFactory]::CreateClient($connection)

    return [pscustomobject]@{
        Client = $client
        Connection = $connection
    }
}

function Invoke-WithModbusSession {
    param(
        [string]$Target,
        [int]$Port,
        [byte]$SlaveId,
        [int]$TimeoutMs,
        [string]$Transport,
        [int]$Baud,
        [scriptblock]$Operation
    )

    $session = $null
    try {
        $session = New-ModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud
        return & $Operation $session
    }
    finally {
        if ($null -ne $session -and $null -ne $session.Client) {
            $session.Client.Dispose()
        }
    }
}

function New-ModbusReadObject {
    param(
        [string]$Operation,
        [int]$FunctionCode,
        [object]$Connection,
        [byte]$SlaveId,
        [uint16]$Address,
        [uint16]$Count,
        [object[]]$Values
    )

    [pscustomobject]@{
        Request = [pscustomobject]@{
            Operation = $Operation
            FunctionCode = $FunctionCode
            Transport = $Connection.Transport.ToString()
            Target = $Connection.DisplayTarget
            UnitId = [int]$SlaveId
            Address = [int]$Address
            Count = [int]$Count
        }
        Response = [pscustomobject]@{
            Values = $Values
            Count = $Values.Count
            TimestampUtc = [datetime]::UtcNow
        }
    }
}

function New-ModbusWriteObject {
    param(
        [string]$Operation,
        [int]$FunctionCode,
        [object]$Connection,
        [byte]$SlaveId,
        [uint16]$Address,
        [object[]]$Values
    )

    [pscustomobject]@{
        Request = [pscustomobject]@{
            Operation = $Operation
            FunctionCode = $FunctionCode
            Transport = $Connection.Transport.ToString()
            Target = $Connection.DisplayTarget
            UnitId = [int]$SlaveId
            Address = [int]$Address
            Count = $Values.Count
            Values = $Values
        }
        Response = [pscustomobject]@{
            Acknowledged = $true
            TimestampUtc = [datetime]::UtcNow
        }
    }
}

function Get-ModbusCoil {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [uint16]$Count,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $values = $session.Client.ReadCoilsAsync($Address, $Count).GetAwaiter().GetResult()
        New-ModbusReadObject -Operation 'ReadCoils' -FunctionCode 1 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Count $Count -Values @($values)
    }
}

function Get-ModbusDiscreteInput {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [uint16]$Count,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $values = $session.Client.ReadDiscreteInputsAsync($Address, $Count).GetAwaiter().GetResult()
        New-ModbusReadObject -Operation 'ReadDiscreteInputs' -FunctionCode 2 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Count $Count -Values @($values)
    }
}

function Get-ModbusHoldingRegister {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [uint16]$Count,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $values = $session.Client.ReadHoldingRegistersAsync($Address, $Count).GetAwaiter().GetResult()
        New-ModbusReadObject -Operation 'ReadHoldingRegisters' -FunctionCode 3 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Count $Count -Values @($values)
    }
}

function Get-ModbusInputRegister {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [uint16]$Count,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $values = $session.Client.ReadInputRegistersAsync($Address, $Count).GetAwaiter().GetResult()
        New-ModbusReadObject -Operation 'ReadInputRegisters' -FunctionCode 4 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Count $Count -Values @($values)
    }
}

function Set-ModbusCoil {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [bool]$Value,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $session.Client.WriteSingleCoilAsync($Address, $Value).GetAwaiter().GetResult()
        New-ModbusWriteObject -Operation 'WriteSingleCoil' -FunctionCode 5 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Values @($Value)
    }
}

function Set-ModbusRegister {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [uint16]$Value,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $session.Client.WriteSingleRegisterAsync($Address, $Value).GetAwaiter().GetResult()
        New-ModbusWriteObject -Operation 'WriteSingleRegister' -FunctionCode 6 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Values @($Value)
    }
}

function Set-ModbusCoils {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [bool[]]$Values,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $session.Client.WriteMultipleCoilsAsync($Address, $Values).GetAwaiter().GetResult()
        New-ModbusWriteObject -Operation 'WriteMultipleCoils' -FunctionCode 15 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Values @($Values)
    }
}

function Set-ModbusRegisters {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [uint16]$Address,

        [Parameter(Mandatory = $true)]
        [uint16[]]$Values,

        [Alias('Host')]
        [string]$Target = 'localhost',

        [ValidateSet('auto', 'tcp', 'tcp/ip', 'rtu-tcp', 'tcp/ip-rtu', 'rtu-serial', 'serial')]
        [string]$Transport = 'auto',

        [int]$Port = 502,
        [byte]$SlaveId = 1,
        [int]$TimeoutMs = 5000,
        [int]$Baud = 9600
    )

    Invoke-WithModbusSession -Target $Target -Port $Port -SlaveId $SlaveId -TimeoutMs $TimeoutMs -Transport $Transport -Baud $Baud -Operation {
        param($session)
        $session.Client.WriteMultipleRegistersAsync($Address, $Values).GetAwaiter().GetResult()
        New-ModbusWriteObject -Operation 'WriteMultipleRegisters' -FunctionCode 16 -Connection $session.Connection -SlaveId $SlaveId -Address $Address -Values @($Values)
    }
}

Export-ModuleMember -Function @(
    'Get-ModbusCoil',
    'Get-ModbusDiscreteInput',
    'Get-ModbusHoldingRegister',
    'Get-ModbusInputRegister',
    'Set-ModbusCoil',
    'Set-ModbusRegister',
    'Set-ModbusCoils',
    'Set-ModbusRegisters'
)
