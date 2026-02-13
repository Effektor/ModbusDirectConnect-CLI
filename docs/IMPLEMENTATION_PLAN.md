# mbdc Implementation Plan

## Goals
- Ship a cross-platform Modbus CLI named `mbdc`.
- Keep invocation fast for field use with flat option-based syntax.
- Support TCP and RTU-over-TCP now, serial when library API supports it.
- Maintain CI quality gates and distribution readiness.

## Current State
- CLI syntax is flag-based per `Commands.md` (no subcommand compatibility path).
- Implemented operations:
  - Reads: coil, discrete, holding, input register
  - Writes: single/multi coil, single/multi register (where supported by library API)
- Transport inference supports positional target and `--serial`/`--rtu` controls.
- Package API constraint remains for direct serial runtime support in `ModbusDirectConnect` `1.1.1` (`net8.0`).

## Phase 1 (Done)
- Flat parser and operation routing
- Endpoint/transport inference
- Verbosity handling
- Real client adapter for supported transports
- Unit test scaffolding + CI build/test flow

## Phase 2 (Next)
- Add serial runtime path when public serial channel API is available
- Add integration tests against simulated Modbus endpoints

## Phase 3
- Finalize package/distribution automation for Chocolatey, WinGet, apt, and PowerShell module workflows
