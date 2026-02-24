# Changelog

## [2.3.2](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.3.1...v2.3.2) (2026-02-24)


### Miscellaneous Chores

* trigger ci to publish ([f6dd6df](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/f6dd6df96355003c7c00c1f046f96536e538d62f))

## [2.3.1](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.3.0...v2.3.1) (2026-02-24)


### Bug Fixes

* modify release-please workflow ([0d70fce](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/0d70fce363266abfbf0f84d29139614c65615dca))

## [2.3.0](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.3.0-rc.4...v2.3.0) (2026-02-24)


### Miscellaneous Chores

* release v2.3.0 ([06ab9af](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/06ab9af91fe8ac7bef322032b30e3d548f7aeff2))

## [2.3.0-rc.4](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.3.0-rc.3...v2.3.0-rc.4) (2026-02-22)


### Bug Fixes

* Make scan hot/cold cadence explicit and incremental ([deef1cc](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/deef1cc895f52a5bfbd8b590e03096a9290ff817))

## [2.3.0-rc.3](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.3.0-rc.2...v2.3.0-rc.3) (2026-02-22)


### Bug Fixes

* baseline scan values before tracking changes ([534440d](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/534440dcef3b374bcda44fcd440b8062893bf8d2))
* baseline scan values before tracking changes ([41cab4e](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/41cab4e50cf1ffb6843f55300038560fa3c3cdb6))

## [2.3.0-rc.2](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.3.0-rc.1...v2.3.0-rc.2) (2026-02-22)


### Features

* add dual-rate scan polling for volatile addresses ([f88235d](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/f88235d5876edf71ee65638649407fddad4fa7e4))
* make scan non-blocking with dual-rate polling ([4caed80](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/4caed806842187b5c6f6f2879ccb0d4e26a6e7c9))
* require explicit --baud for serial transport ([4e2865b](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/4e2865ba7b905ee726fec176fbb71e4c975501db))
* require explicit --baud for serial transport ([9836d23](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/9836d23ed325744b1fc57df4e71a9acd5d90771c))
* run scan discovery in background and expand monitored ranges ([492410c](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/492410cea01574af371fed3654d36300e7e475e9))


### Bug Fixes

* use max FC probe windows and harden analyze/scan reads ([640829a](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/640829ae0044a642a6bf4fff4de5fe45c3388b2d))

## [2.3.0-rc.1](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.3.0-rc...v2.3.0-rc.1) (2026-02-22)


### Features

* add bash tab completion in deb package ([b0990e3](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/b0990e31f911ac7c43a883e3accab1df90804c4e))
* add bash tab completion to deb install ([16dda85](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/16dda853a7d46abe04ff57c63d123626d8361a46))


### Bug Fixes

* classify probe errors by transport and tolerate frame failures ([c401a71](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/c401a7101bb7b68ebc24a4e5473b5b5abe4264aa))
* keep analyze probing resilient and transport-aware ([db55eb8](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/db55eb8b924fec614bc05ff63b38aeafab97502e))

## [2.3.0-rc](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.2...v2.3.0-rc) (2026-02-22)


### Features

* add analyze mode and live multi-FC scan dashboard ([198d728](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/198d728e4cbd4de77d2bf1e096cb8da83a98c924))
* add analyze/scan modes and enforce read limits ([d0360cf](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/d0360cfa029c85cc8f7f011feffec9b86150c96b))

## [2.2.2](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.2...v2.2.2) (2026-02-18)


### ⚠ BREAKING CHANGES

* Added deb pkg config

### Features

* Added deb pkg config ([24505f2](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/24505f2dc5b997162d513be4900f2053278529a8))
* **ci:** add amd64/arm64 binary and deb release artifacts ([04decca](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/04deccab97d72fc2a38ecafcc80af4d9faa2622f))
* **ci:** publish amd64/arm64 binaries and deb packages ([d7e72bf](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/d7e72bf8f508ade52d4a4d1e01c3d2d26920a0c8))
* **serial:** implement RTU over serial ([8421646](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/8421646353398bf17904bf4d67847a957fdd8092))
* **serial:** implement RTU over serial ([f6f47ac](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/f6f47ac27058215b46dfe69a051a5827976281b8))


### Bug Fixes

* Added test file ([6fdbc43](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/6fdbc43e37e4b4c81ffac569884062e600ae5fb1))
* **ci:** dedupe build/test triggers and fix deb packaging ([1f714b9](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/1f714b92bc00fa99e377a754f70482267afeed35))
* **ci:** harden release-please token selection ([10adc71](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/10adc71ed94d04cf551b2b10e3e4b11617fa64c8))
* **ci:** pass repo context to gh release upload ([13f66ff](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/13f66ff34fb6dabbe3bb0b41f7755334a25f9013))
* **ci:** provide repository context for release asset upload ([82917be](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/82917be4fcea5443425944feac1e73aae7b2e002))
* **ci:** remove duplicate build triggers and replace nfpm action ([34fe131](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/34fe13157f91fd739d3ac8b2a526feee4981e264))
* **ci:** skip build-and-test workflow on main pushes ([a7b1563](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/a7b1563421ac2f8eea6f173b74ed75c2750e16c7))
* **ci:** validate release-please PAT access before use ([7d8543e](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/7d8543ed7fab0c761ac7028e45add585f49cc045))
* **linux:** ship System.IO.Ports native lib ([6914c21](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/6914c21207398e4841f84edd3a95d8afa7554cc8))
* **linux:** ship System.IO.Ports native lib ([027cd13](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/027cd130d4d8ad1d71199acd397f7017deb645eb))
* prevent -rh null reference by decoding holding registers from Data ([9a6756d](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/9a6756defe8f28e16e7c1abf6a8a90c839904c80))
* prevent null-reference failures on register reads ([24a665c](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/24a665c821abee0f5de444dcd28379a1fa761298))


### Miscellaneous Chores

* release v2.2.2 ([99c8f6a](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/99c8f6a28290b21720a68999aaafa0aec4061e42))

## [2.2.2](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.2...v2.2.2) (2026-02-18)


### ⚠ BREAKING CHANGES

* Added deb pkg config

### Features

* Added deb pkg config ([24505f2](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/24505f2dc5b997162d513be4900f2053278529a8))
* **ci:** add amd64/arm64 binary and deb release artifacts ([04decca](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/04deccab97d72fc2a38ecafcc80af4d9faa2622f))
* **ci:** publish amd64/arm64 binaries and deb packages ([d7e72bf](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/d7e72bf8f508ade52d4a4d1e01c3d2d26920a0c8))
* **serial:** implement RTU over serial ([8421646](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/8421646353398bf17904bf4d67847a957fdd8092))
* **serial:** implement RTU over serial ([f6f47ac](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/f6f47ac27058215b46dfe69a051a5827976281b8))


### Bug Fixes

* Added test file ([6fdbc43](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/6fdbc43e37e4b4c81ffac569884062e600ae5fb1))
* **ci:** dedupe build/test triggers and fix deb packaging ([1f714b9](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/1f714b92bc00fa99e377a754f70482267afeed35))
* **ci:** harden release-please token selection ([10adc71](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/10adc71ed94d04cf551b2b10e3e4b11617fa64c8))
* **ci:** pass repo context to gh release upload ([13f66ff](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/13f66ff34fb6dabbe3bb0b41f7755334a25f9013))
* **ci:** provide repository context for release asset upload ([82917be](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/82917be4fcea5443425944feac1e73aae7b2e002))
* **ci:** remove duplicate build triggers and replace nfpm action ([34fe131](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/34fe13157f91fd739d3ac8b2a526feee4981e264))
* **ci:** skip build-and-test workflow on main pushes ([a7b1563](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/a7b1563421ac2f8eea6f173b74ed75c2750e16c7))
* **ci:** validate release-please PAT access before use ([7d8543e](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/7d8543ed7fab0c761ac7028e45add585f49cc045))
* **linux:** ship System.IO.Ports native lib ([6914c21](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/6914c21207398e4841f84edd3a95d8afa7554cc8))
* **linux:** ship System.IO.Ports native lib ([027cd13](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/027cd130d4d8ad1d71199acd397f7017deb645eb))
* prevent -rh null reference by decoding holding registers from Data ([9a6756d](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/9a6756defe8f28e16e7c1abf6a8a90c839904c80))
* prevent null-reference failures on register reads ([24a665c](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/24a665c821abee0f5de444dcd28379a1fa761298))


### Miscellaneous Chores

* release v2.2.2 ([99c8f6a](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/99c8f6a28290b21720a68999aaafa0aec4061e42))

## [2.2.2](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.2-rc...v2.2.2) (2026-02-18)


### Miscellaneous Chores

* release v2.2.2 ([99c8f6a](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/99c8f6a28290b21720a68999aaafa0aec4061e42))

## [2.2.2-rc](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.1...v2.2.2-rc) (2026-02-17)


### Bug Fixes

* Added test file ([6fdbc43](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/6fdbc43e37e4b4c81ffac569884062e600ae5fb1))
* prevent -rh null reference by decoding holding registers from Data ([9a6756d](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/9a6756defe8f28e16e7c1abf6a8a90c839904c80))
* prevent null-reference failures on register reads ([24a665c](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/24a665c821abee0f5de444dcd28379a1fa761298))

## [2.2.1](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.0...v2.2.1) (2026-02-14)


### Bug Fixes

* **linux:** ship System.IO.Ports native lib ([6914c21](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/6914c21207398e4841f84edd3a95d8afa7554cc8))
* **linux:** ship System.IO.Ports native lib ([027cd13](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/027cd130d4d8ad1d71199acd397f7017deb645eb))

## [2.2.0](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.1.1...v2.2.0) (2026-02-14)


### Features

* **serial:** implement RTU over serial ([8421646](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/8421646353398bf17904bf4d67847a957fdd8092))
* **serial:** implement RTU over serial ([f6f47ac](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/f6f47ac27058215b46dfe69a051a5827976281b8))


### Bug Fixes

* **ci:** skip build-and-test workflow on main pushes ([a7b1563](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/a7b1563421ac2f8eea6f173b74ed75c2750e16c7))

## [2.1.1](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.1.0...v2.1.1) (2026-02-13)


### Bug Fixes

* **ci:** dedupe build/test triggers and fix deb packaging ([1f714b9](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/1f714b92bc00fa99e377a754f70482267afeed35))
* **ci:** remove duplicate build triggers and replace nfpm action ([34fe131](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/34fe13157f91fd739d3ac8b2a526feee4981e264))

## [2.1.0](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.0.1...v2.1.0) (2026-02-13)


### Features

* **ci:** add amd64/arm64 binary and deb release artifacts ([04decca](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/04deccab97d72fc2a38ecafcc80af4d9faa2622f))
* **ci:** publish amd64/arm64 binaries and deb packages ([d7e72bf](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/d7e72bf8f508ade52d4a4d1e01c3d2d26920a0c8))

## [2.0.1](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.0.0...v2.0.1) (2026-02-13)


### Bug Fixes

* **ci:** pass repo context to gh release upload ([13f66ff](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/13f66ff34fb6dabbe3bb0b41f7755334a25f9013))
* **ci:** provide repository context for release asset upload ([82917be](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/82917be4fcea5443425944feac1e73aae7b2e002))

## [2.0.0](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v1.0.0...v2.0.0) (2026-02-13)


### ⚠ BREAKING CHANGES

* Added deb pkg config

### Features

* Added deb pkg config ([24505f2](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/24505f2dc5b997162d513be4900f2053278529a8))


### Bug Fixes

* **ci:** harden release-please token selection ([10adc71](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/10adc71ed94d04cf551b2b10e3e4b11617fa64c8))
* **ci:** validate release-please PAT access before use ([7d8543e](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/7d8543ed7fab0c761ac7028e45add585f49cc045))
