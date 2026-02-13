# mbdc

`mbdc` is a command-line Modbus client for quick reads, writes, and monitoring.

It is built for fast troubleshooting and day-to-day field usage.

## Download

Download prebuilt binaries from GitHub Releases:
- https://github.com/Effektor/ModbusDirectConnect-CLI/releases

Release assets:
- `mbdc-linux-amd64.tar.gz`
- `mbdc-linux-arm64.tar.gz`
- `mbdc-macos-x64.tar.gz`
- `mbdc-windows-x64.zip`
- `mbdc_<version>_amd64.deb`
- `mbdc_<version>_arm64.deb`
- `ModbusDirectConnect-PowerShell.zip` (PowerShell module package)

## Install

### Linux
```bash
# for x86_64 / amd64 use mbdc-linux-amd64.tar.gz
# for ARM64 use mbdc-linux-arm64.tar.gz
tar -xzf mbdc-linux-amd64.tar.gz
chmod +x mbdc
sudo mv mbdc /usr/local/bin/
mbdc --version
```

### macOS
```bash
tar -xzf mbdc-macos-x64.tar.gz
chmod +x mbdc
sudo mv mbdc /usr/local/bin/
mbdc --version
```

### Windows
1. Extract `mbdc-windows-x64.zip`
2. Put `mbdc.exe` in a folder on your `PATH`
3. Run:
```powershell
mbdc --version
```

## Command Pattern

```bash
mbdc [TARGET] [TRANSPORT OPTIONS] [OPERATION] [DATA/OUTPUT OPTIONS]
```

- `TARGET`: `host`, `host:port`, `/dev/tty...`, `COM...`
- Exactly one operation is required per command.
- `SPEC` accepts `ADDR` or `ADDR:COUNT`.

## What Works Today

- Modbus TCP reads/writes
- Modbus RTU-over-TCP reads/writes
- Read operations:
  - `--read-coil`
  - `--read-discrete`
  - `--read-holding`
  - `--read-inputreg`
  - `--ref` (Modicon reference addressing)
- Register write operations:
  - `--write-reg`
  - `--write-multi-reg`
- Monitoring modes:
  - `--watch [SEC]`
  - `--monitor [SEC]`

## Quick Examples

### Read coils (TCP)
```bash
mbdc 192.168.1.10 --read-coil 0:8
```

### Read holding registers using Modicon reference
```bash
mbdc 192.168.1.10 --ref 40001:10
```

### Read via RTU-over-TCP
```bash
mbdc 192.168.1.10 --rtu --read-holding 0:6
```

### Watch values continuously
```bash
mbdc 192.168.1.10 --read-inputreg 0:20 --watch 0.5 --only-changed
```

### Decode register bytes as string
```bash
mbdc 192.168.1.10 --read-holding 100:16 --string --string-len 32 --null-term --encoding iso-8859-1
```

### Write a single holding register
```bash
mbdc 192.168.1.10 --write-reg 2 --data 0x1234
```

### Write multiple holding registers
```bash
mbdc 192.168.1.10 --write-multi-reg 10 --data 1,2,3,4
```

## Output Controls

- Verbosity: `-v`, `-vv`, `-vvv`
- Minimal output: `--quiet`
- JSON output: `--json`
- Timestamps: `--timestamp`
- Bit/hex views for coil/discrete reads: `--bits`, `--hex`

## PowerShell Module

A PowerShell module is available as `ModbusDirectConnect-PowerShell.zip` in Releases.

It provides cmdlets such as:
- `Get-ModbusCoil`
- `Get-ModbusDiscreteInput`
- `Get-ModbusHoldingRegister`
- `Get-ModbusInputRegister`
- `Set-ModbusRegister`
- `Set-ModbusRegisters`

## Current Limitations

- RTU over serial is not available yet in runtime (`--serial` targets are parsed, but runtime support is pending).
- Coil write routes are parsed (`--write-coil`, `--write-multi-coil`) but not available in the current library runtime.
- `--ascii` is reserved and currently returns not implemented.
- Some advanced decode/format flags are accepted as milestones but are not fully implemented yet.

## Help

For the full up-to-date command reference from the binary itself:

```bash
mbdc --help
```

## License

GPL-3.0. See `LICENSE`.
