# mbdc CLI Syntax

## Core Pattern
```bash
mbdc [target] [global-options] <operation> [operation-options]
```

`target` can be an IP/host or serial device path and is inferred when the first token is not an option.

## Read Operations
- `--read-coil <address>`, `--rc <address>`, `-rc <address>`
- `--read-discrete <address>`, `--rd <address>`, `-rd <address>`
- `--read-holding <address>`, `--rh <address>`, `-rh <address>`
- `--read-inputreg <address>`, `--ri <address>`, `-ri <address>`
- `--count <n>`, `-c <n>`

## Write Operations
- `--write-coil <address>`, `--wc <address>`, `-wc <address>` + `--data <value>`, `-d <value>`
- `--write-reg <address>`, `--wr <address>`, `-wr <address>` + `--data <value>`, `-d <value>`
- `--write-multi-coil <address>` + `--data <payload>`, `-d <payload>`
- `--write-multi-reg <address>` + `--data <payload>`, `-d <payload>`

Only one operation flag may be used per command invocation.

## Transport Options
- `--rtu` use RTU framing (RTU-over-TCP for network targets)
- `--serial <port>` force serial target (e.g. `COM5`, `/dev/serial1`)
- `--ascii` Modbus ASCII mode (reserved; not yet implemented)

## General Options
- `--port <port>`, `-p <port>` default `502`
- `--slave <id>`, `-s <id>` default `1`
- `--timeout <ms>`, `-t <ms>` default `5000`
- `-v`, `-vv`, `-vvv` verbosity levels

## Examples
```bash
# Read from TCP slave
mbdc 192.168.1.1 --read-coil 1 --count 5

# Read from RTU-over-TCP slave
mbdc 192.168.1.1 --rtu --read-coil 1 --count 5

# Read from serial on Windows
mbdc --serial COM5 --read-coil 1 --count 5

# Read from serial on Linux
mbdc /dev/serial1 --read-coil 1 --count 5

# Read from serial in ASCII mode (not yet implemented)
mbdc /dev/serial1 --ascii --rc 2 -c 5

# Write single coil
mbdc 192.168.1.1 --write-coil 2 --data 0x0014

# Write single register
mbdc 192.168.1.1 --write-reg 2 --data 0x1234

# Write multiple coils
mbdc 192.168.1.1 --write-multi-coil 2 --data 1234

# Write multiple registers
mbdc 192.168.1.1 --write-multi-reg 2 --data 0xFFFF
```
