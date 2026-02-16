# Changelog

## [2.2.3](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.2...v2.2.3) (2026-02-16)


### Bug Fixes

* **ci:** parse conventional PR titles and dispatch publishers from stable release ([689ecaa](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/689ecaa5ff94a7e16ef8021b98bf98ba2c4536d5))
* **ci:** parse PR titles and stop duplicate publisher triggers ([16e5b45](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/16e5b45315d9ea8a058d1888ec563de373587b84))
* **ci:** publish rc binaries and support Release-tag stable flow ([757a059](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/757a059b2abc1c36e54a80c8c6b369a8f8a80d56))
* **ci:** publish rc binaries and trigger stable release from Release tag ([9c07d2f](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/9c07d2fd0e626a3f2e61a4256435bb1f80745aec))
* **ci:** release stable from release branch and dispatch publishers ([715bf17](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/715bf17930fe7442aef6e5b62abe8dc96df56477))
* **ci:** revert invalid release-please pattern config ([#26](https://github.com/Effektor/ModbusDirectConnect-CLI/issues/26)) ([2a17740](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/2a17740969314d9bee8899901461b9d83f02068f))
* **ci:** support release-branch promotion to stable binaries ([fbeb399](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/fbeb399e7f0120db3b86c8dcc04ca91e9d79ee1a))
* **release:** document rc and stable promotion branches ([05d80c3](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/05d80c3e5b6489727a9f525c626e1b1750d2b538))
* **release:** document rc and stable promotion branches ([383bb03](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/383bb03718b9793f93d31bbed7dea0a10950ca03))
* **release:** simplify release-please rc and stable promotion flows ([#27](https://github.com/Effektor/ModbusDirectConnect-CLI/issues/27)) ([9a4aa3f](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/9a4aa3f792e8ff5c54773aec9985b1c7f951ce2a))

## [2.2.2](https://github.com/Effektor/ModbusDirectConnect-CLI/compare/v2.2.1...v2.2.2) (2026-02-16)


### Bug Fixes

* **ci:** enforce rc tag suffix for prereleases ([57cd763](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/57cd763c19a1173349d19e60faf49b05f83811ff))
* **ci:** enforce rc-tag prereleases on main ([d679b21](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/d679b213a3db55efaf8eb6360c8b776935839b7c))
* **ci:** prerelease-on-main + stable release pipeline ([5957ee6](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/5957ee6b0888fdd48aba6c1721095509da0f1832))
* **ci:** split prerelease and stable release pipelines ([a4bc105](https://github.com/Effektor/ModbusDirectConnect-CLI/commit/a4bc10599f0735a761876f8c4f445c7442916e2b))

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
