# mbdc-cli Implementation Plan

## Goals
- Build a cross-platform Modbus CLI around the `ModbusDirectConnect` NuGet package.
- Provide Linux-style CLI ergonomics (`--long`, `-s`, predictable defaults).
- Keep command usage fast for field work (transport inference from target input).
- Support strong output controls (minimal, verbose, register text decoding).
- Establish repeatable quality gates (unit tests + CI).

## Current Baseline
- Existing scaffold has `read` and `write` subcommands.
- The referenced package version (`ModbusDirectConnect` `1.1.1`) exposes public APIs for:
  - Modbus TCP (`ModbusTCPChannel`)
  - Modbus RTU over TCP (`RtuTCPChannel`)
  - Read operations: coils, discrete inputs, input registers, holding registers
  - Write operations: holding register (single)
- The same package version does **not** expose a public serial channel API for direct RTU-over-serial in `net8.0`.

## Delivery Phases

## Phase 1: CLI Plumbing Baseline (this iteration)
- Add normalized endpoint/transport parsing:
  - Accept endpoint as first positional token: `mbdc-cli <target> ...`
  - Infer transport from target when protocol is omitted.
- Add verbosity controls:
  - `-v` summary metadata
  - `-vv` request/response context
  - `-vvv` include transport diagnostics
- Add read output controls:
  - Scalar output modes for registers (`u16`, `hex`)
  - Text decode modes (`ascii`, `utf8`, `cstring`)
- Add continuous read mode:
  - `--watch` with `--interval-ms`
  - `--same-line` for monitor mode.
- Wire supported runtime transports:
  - TCP
  - RTU over TCP
- Add clear errors for unsupported combinations:
  - RTU over serial execution path (pending library support)
  - write operations missing in current package API

## Phase 2: Serial Enablement (blocked by library API surface)
- Integrate RTU-over-serial once the library exposes public serial channel types/configuration in the NuGet package for `net8.0`.
- Add serial-specific options and validation:
  - `--baud`, `--data-bits`, `--parity`, `--stop-bits`
- Add integration tests against loopback/simulated serial endpoints.

## Phase 3: Packaging and Distribution
- Prepare distro artifacts and metadata for:
  - Chocolatey
  - WinGet
  - apt (deb repo or package)
  - PowerShell `Install-Module`
- Add release automation for versioning and checksums.

## Test Strategy
- Unit tests for:
  - endpoint parsing/inference
  - verbosity token parsing (`-v/-vv/-vvv`)
  - register formatting/decoding
  - monitor rendering mode selection
- Integration tests (later):
  - mock Modbus server for TCP / RTU-over-TCP roundtrips

## CI Strategy
- Build + test on `ubuntu`, `windows`, `macos`.
- Fail PRs on test failures.
- Keep release job tag-driven (`v*`) and publish self-contained binaries.

## Open Risks
- Serial transport support depends on package API availability.
- Package restore to private GitHub feed requires valid auth in CI and release runners.
