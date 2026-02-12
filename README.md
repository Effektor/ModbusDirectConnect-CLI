# ModbusDirectConnect-CLI

CLI tool for Modbus TCP/RTU using the ModbusDirectConnect library.

## Build

```bash
cd ModbusDirectConnect.CLI
dotnet build
```

GitHub Packages auth required:
```bash
dotnet nuget add source https://nuget.pkg.github.com/effektor/index.json \
  --name effektor --username USER --password TOKEN --store-password-in-clear-text
```

## Usage

```bash
modbus-cli read holding-registers 1000 10 -H 192.168.1.100
modbus-cli write register 1000 42 -H 192.168.1.100
```
