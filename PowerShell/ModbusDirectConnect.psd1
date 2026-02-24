@{
    # Script module or binary module file associated with this manifest
    RootModule = 'ModbusDirectConnect.psm1'
    
    # Version number of this module
    ModuleVersion = '2.3.1-rc' # x-release-please-version
    
    # ID used to uniquely identify this module
    GUID = 'a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d'
    
    # Author of this module
    Author = 'Effektor'
    
    # Company or vendor of this module
    CompanyName = 'Effektor'
    
    # Copyright statement for this module
    Copyright = '(c) 2026 Effektor. All rights reserved.'
    
    # Description of the functionality provided by this module
    Description = 'PowerShell module for ModbusDirectConnect CLI - A command-line tool for Modbus communication'
    
    # Minimum version of the PowerShell engine required by this module
    PowerShellVersion = '5.1'
    
    # Functions to export from this module
    FunctionsToExport = @(
        'Get-ModbusCoil',
        'Get-ModbusDiscreteInput',
        'Get-ModbusHoldingRegister',
        'Get-ModbusInputRegister',
        'Set-ModbusCoil',
        'Set-ModbusRegister',
        'Set-ModbusCoils',
        'Set-ModbusRegisters'
    )
    
    # Cmdlets to export from this module
    CmdletsToExport = @()
    
    # Variables to export from this module
    VariablesToExport = @()
    
    # Aliases to export from this module
    AliasesToExport = @()
    
    # Private data to pass to the module specified in RootModule/ModuleToProcess
    PrivateData = @{
        PSData = @{
            # Tags applied to this module
            Tags = @('Modbus', 'TCP', 'RTU', 'Industrial', 'Automation', 'CLI')
            
            # A URL to the license for this module
            LicenseUri = 'https://github.com/Effektor/ModbusDirectConnect-CLI/blob/main/LICENSE'
            
            # A URL to the main website for this project
            ProjectUri = 'https://github.com/Effektor/ModbusDirectConnect-CLI'
            
            # ReleaseNotes of this module
            ReleaseNotes = 'Initial release of ModbusDirectConnect PowerShell module'
        }
    }
}
