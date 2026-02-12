# ModbusDirectConnect-CLI

Cross-platform CLI wrapper around the `ModbusDirectConnect` NuGet package.

## Executable
- CLI binary name: `mbdc`

## Status
- Implemented: flat flag-based CLI syntax from `Commands.md`
- Implemented: Modbus TCP and Modbus RTU-over-TCP operations
- Pending: direct RTU-over-serial runtime support (blocked by current public API surface in `ModbusDirectConnect` `1.1.1` for `net8.0`)

## Build
```bash
dotnet restore ModbusDirectConnect.CLI.sln
dotnet build ModbusDirectConnect.CLI.sln
```

## Test
```bash
dotnet test ModbusDirectConnect.CLI.sln
```

## Quick Usage
```bash
# Read coils from TCP
mbdc 192.168.1.1 --read-coil 1 --count 5

# Read coils from RTU-over-TCP
mbdc 192.168.1.1 --rtu --read-coil 1 --count 5

# Read coils from serial port (inferred)
mbdc /dev/serial1 --read-coil 1 --count 5
mbdc --serial COM5 --read-coil 1 --count 5

# Write single register
mbdc 192.168.1.1 --write-reg 2 --data 0x1234
```

## Documentation
- CLI syntax: `docs/CLI_SYNTAX.md`
- Implementation plan: `docs/IMPLEMENTATION_PLAN.md`
