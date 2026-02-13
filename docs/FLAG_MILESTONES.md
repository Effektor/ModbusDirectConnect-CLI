# Flag Milestones

`HELP.md` is the technical spec. This file tracks implementation progress for each long flag without blocking releases while features are still in progress.

Status values:
- `Implemented`: behavior is implemented in CLI runtime
- `Partial`: flag is parsed and/or partially honored, but not fully per spec
- `Planned`: reserved milestone, not implemented yet

| Flag | Status | Notes |
| --- | --- | --- |
| `--ascii` | Planned | Reserved; runtime currently returns not implemented. |
| `--baud` | Partial | Parsed and stored in connection options; blocked by serial transport support in library. |
| `--be` | Planned | Parsing milestone only. |
| `--bits` | Partial | Implemented for bit output and multi-coil payload parsing; advanced behavior still pending. |
| `--bool` | Implemented | Coil/discrete true/false output mode. |
| `--bs` | Planned | Parsing milestone only. |
| `--byte-swap` | Planned | Parsing milestone only. |
| `--bytes` | Partial | Implemented for register byte output; write-byte packing is basic. |
| `--count` | Implemented | Read count supported; overridden by `SPEC` suffix count. |
| `--data` | Implemented | Core write payload input. |
| `--databits` | Partial | Parsed and stored; serial runtime pending. |
| `--diff` | Partial | Basic changed-line highlighting in watch/monitor loop. |
| `--encoding` | Implemented | String decode encoding selector. |
| `--f32` | Planned | Parsing milestone only. |
| `--f64` | Planned | Parsing milestone only. |
| `--group` | Planned | Parsing milestone only. |
| `--help` | Implemented | Prints technical help text from `HELP.md` resource. |
| `--hex` | Implemented | Hex output for packed bit reads. |
| `--json` | Partial | Basic JSON payload output for reads. |
| `--le` | Planned | Parsing milestone only. |
| `--little-endian` | Planned | Parsing milestone only. |
| `--monitor` | Implemented | Continuous redraw mode. |
| `--no-color` | Planned | Parsing milestone only. |
| `--null-term` | Implemented | Null-terminated string decode behavior. |
| `--off` | Implemented | Convenience single-coil write OFF. |
| `--on` | Implemented | Convenience single-coil write ON. |
| `--only-changed` | Partial | Filters watch/monitor output to changed lines. |
| `--parity` | Partial | Parsed and stored; serial runtime pending. |
| `--port` | Implemented | TCP port selection. |
| `--quiet` | Implemented | Minimal line output mode. |
| `--raw-bytes` | Planned | Parsing milestone only. |
| `--raw16` | Implemented | Raw 16-bit write payload helper. |
| `--read-coil` | Implemented | FC01 read operation. |
| `--read-discrete` | Implemented | FC02 read operation. |
| `--read-holding` | Implemented | FC03 read operation. |
| `--read-inputreg` | Implemented | FC04 read operation. |
| `--ref` | Implemented | Modicon ref addressing translation to FC/address. |
| `--retries` | Planned | Parsing milestone only. |
| `--rtu` | Partial | RTU-over-TCP supported; RTU-serial blocked by library API. |
| `--s16` | Planned | Parsing milestone only. |
| `--s32` | Planned | Parsing milestone only. |
| `--scan` | Planned | Parsing milestone only. |
| `--serial` | Partial | Inference and parsing implemented; runtime blocked by serial API availability. |
| `--stopbits` | Partial | Parsed and stored; serial runtime pending. |
| `--string` | Implemented | Register-byte string decoding mode. |
| `--string-len` | Implemented | String byte-length limiter. |
| `--tcp` | Implemented | Force Modbus/TCP transport selection. |
| `--timeout` | Implemented | Timeout in seconds accepted and mapped to ms internally. |
| `--timestamp` | Implemented | Prefix output lines with timestamp. |
| `--u16` | Planned | Parsing milestone only. |
| `--u32` | Planned | Parsing milestone only. |
| `--unit` | Implemented | Unit/slave ID global option. |
| `--value` | Implemented | Alias of `--data`. |
| `--version` | Implemented | Prints assembly version. |
| `--watch` | Implemented | Continuous append read mode. |
| `--word-swap` | Planned | Parsing milestone only. |
| `--write-coil` | Implemented | FC05 write route. |
| `--write-multi-coil` | Partial | Route + payload parsing implemented; depends on library API support. |
| `--write-multi-reg` | Implemented | FC16-style route (implemented as sequential writes with current library API). |
| `--write-reg` | Implemented | FC06 write route. |
| `--ws` | Planned | Parsing milestone only. |
