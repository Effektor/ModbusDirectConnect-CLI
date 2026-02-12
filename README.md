# ModbusDirectConnect-CLI

Cross-platform CLI wrapper around the `ModbusDirectConnect` NuGet package.

## Status
- Implemented: Modbus TCP and Modbus RTU over TCP read/write plumbing.
- Implemented: transport inference, verbosity (`-v/-vv/-vvv`), register text decoding, watch/monitor mode.
- Pending: direct RTU over serial runtime support (blocked by public serial channel API availability in `ModbusDirectConnect` `1.1.1` for `net8.0`).

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
# TCP inferred from target
modbus-cli 192.168.1.100 read holding-registers 1000 10

# Explicit RTU over TCP
modbus-cli --protocol rtu-tcp 192.168.1.100:502 read input-registers 0 8 --format utf8

# Verbose with transport diagnostics
modbus-cli -vvv 192.168.1.100 read coils 0 16

# Continuous monitor output
modbus-cli 192.168.1.100 read holding-registers 600 16 --format cstring --watch --same-line --interval-ms 250
```

## Documentation
- CLI syntax: `docs/CLI_SYNTAX.md`
- Implementation plan: `docs/IMPLEMENTATION_PLAN.md`

## NuGet Auth (GitHub Packages)
If your environment does not already have package feed auth configured:

```bash
dotnet nuget add source https://nuget.pkg.github.com/effektor/index.json \
  --name effektor --username USER --password TOKEN --store-password-in-clear-text
```
