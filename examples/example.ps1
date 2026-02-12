# Example PowerShell script demonstrating various Modbus operations

# Configuration
$ModbusHost = "192.168.1.100"
$ModbusPort = 502
$ModbusSlaveId = 1

Write-Host "=== ModbusDirectConnect PowerShell Module Examples ===" -ForegroundColor Cyan
Write-Host ""

# Import the module
Import-Module ModbusDirectConnect

# Example 1: Read coils
Write-Host "1. Reading 10 coils starting at address 0:" -ForegroundColor Yellow
Read-ModbusCoils -Address 0 -Count 10 -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

# Example 2: Read holding registers
Write-Host "2. Reading 5 holding registers starting at address 1000:" -ForegroundColor Yellow
Read-ModbusHoldingRegisters -Address 1000 -Count 5 -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

# Example 3: Write a single coil
Write-Host "3. Writing a single coil at address 0 (value: true):" -ForegroundColor Yellow
Write-ModbusCoil -Address 0 -Value $true -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

# Example 4: Write a single register
Write-Host "4. Writing a single register at address 1000 (value: 12345):" -ForegroundColor Yellow
Write-ModbusRegister -Address 1000 -Value 12345 -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

# Example 5: Write multiple coils
Write-Host "5. Writing multiple coils starting at address 0:" -ForegroundColor Yellow
Write-ModbusCoils -Address 0 -Values @($true, $false, $true, $true, $false) -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

# Example 6: Write multiple registers
Write-Host "6. Writing multiple registers starting at address 1000:" -ForegroundColor Yellow
Write-ModbusRegisters -Address 1000 -Values @(100, 200, 300, 400, 500) -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

# Example 7: Read discrete inputs
Write-Host "7. Reading 8 discrete inputs starting at address 100:" -ForegroundColor Yellow
Read-ModbusDiscreteInputs -Address 100 -Count 8 -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

# Example 8: Read input registers
Write-Host "8. Reading 4 input registers starting at address 2000:" -ForegroundColor Yellow
Read-ModbusInputRegisters -Address 2000 -Count 4 -Host $ModbusHost -Port $ModbusPort -SlaveId $ModbusSlaveId
Write-Host ""

Write-Host "=== Examples completed ===" -ForegroundColor Cyan
