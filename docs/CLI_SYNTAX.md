# mbdc-cli Command Syntax (Draft v1)

## Core Pattern
```bash
mbdc-cli [global-options] [target] <command> <subcommand> [arguments] [options]
```

`target` is optional when `--host` or transport-specific options are provided. If present as the first token and it is not a command, it is treated as endpoint input.

## Commands
- `read`
- `write`

## Read Subcommands
- `read coils <address> <count>`
- `read discrete-inputs <address> <count>`
- `read holding-registers <address> <count>`
- `read input-registers <address> <count>`

## Write Subcommands
- `write coil <address> <value>`
- `write register <address> <value>`
- `write coils <address> <values>`
- `write registers <address> <values>`

## Global Options
- `-P, --protocol <tcp|rtu-tcp|rtu-serial>`
- `-H, --host <host>`
- `-p, --port <port>`
- `-s, --slave <unit-id>`
- `-t, --timeout <ms>`
- `-v`, `-vv`, `-vvv` verbosity levels

## Serial Options
- `--baud <rate>` default `9600`
- `--data-bits <5|6|7|8>` default `8`
- `--parity <none|even|odd>` default `none`
- `--stop-bits <one|two>` default `one`

## Read Output Options
- `-f, --format <u16|hex|ascii|utf8|cstring>`
- `--word-order <be|le>` controls register byte order for text/hex modes
- `--no-header` omit section headers

## Continuous/Monitor Read Options
- `-w, --watch` run continuously
- `--interval-ms <ms>` default `1000`
- `--same-line` rewrite one line (monitor style)

## Transport Inference Rules
When `--protocol` is omitted:
1. If target starts with `/dev/` or matches `COM[0-9]+` -> `rtu-serial`
2. If target starts with `rtu-tcp://` -> `rtu-tcp`
3. If target starts with `tcp://` -> `tcp`
4. Otherwise -> `tcp`

## Examples
```bash
# TCP inferred
mbdc-cli 192.168.1.10 read holding-registers 100 4

# RTU serial inferred
mbdc-cli /dev/ttyUSB0 read input-registers 0 8
mbdc-cli COM3 read coils 10 16

# Explicit RTU over TCP
mbdc-cli rtu-tcp://192.168.1.20:502 read holding-registers 0 6

# UTF-8 decode as text from holding registers
mbdc-cli 192.168.1.10 read holding-registers 600 8 --format utf8

# C-string decode with continuous monitor output on one line
mbdc-cli 192.168.1.10 read holding-registers 600 16 --format cstring --watch --same-line --interval-ms 250

# Transport diagnostics
mbdc-cli -vvv 192.168.1.10 read coils 0 16
```
