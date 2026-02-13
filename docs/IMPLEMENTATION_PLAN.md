# mbdc Implementation Plan

## Source Of Truth
- `HELP.md` defines required CLI syntax, help output, and behavior.
- `docs/FLAG_MILESTONES.md` tracks status of each long flag from `HELP.md`.

## Goals
- Ship a cross-platform Modbus CLI named `mbdc`.
- Keep release throughput high while features are still being built.
- Enforce argument-level quality through broad parser tests.
- Manage versions/releases through `release-please`.
- Keep PowerShell cmdlets on PowerShell conventions as a separate UX surface.

## CI/CD Model
- `.github/workflows/build-and-release.yml`: build + test only (PRs/pushes).
- `.github/workflows/release-please.yml`: release PR/versioning and release asset publishing.

## Delivery Principle
- Features may remain `Planned`/`Partial` without blocking releases.
- Every flag in `HELP.md` must be represented in `docs/FLAG_MILESTONES.md`.
- Unit tests focus heavily on valid CLI argument combinations and parser stability.

## Near-Term Work
1. Implement remaining decode families (`u32/s32/f32/f64`, swaps, scan/group).
2. Implement serial runtime path when public library API supports it.
3. Expand integration tests against Modbus simulators for TCP and RTU-over-TCP.
