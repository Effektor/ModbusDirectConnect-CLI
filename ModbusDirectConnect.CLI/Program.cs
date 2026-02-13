using System.CommandLine;
using ModbusDirectConnect.CLI.Commands;
using ModbusDirectConnect.CLI.Infrastructure;

namespace ModbusDirectConnect.CLI;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("ModbusDirectConnect CLI")
        {
            Description = "Cross-platform CLI tool for reading and writing Modbus devices over TCP and RTU transports."
        };

        var globalOptions = CreateGlobalOptions();

        rootCommand.AddGlobalOption(globalOptions.Target);
        rootCommand.AddGlobalOption(globalOptions.Host);
        rootCommand.AddGlobalOption(globalOptions.Port);
        rootCommand.AddGlobalOption(globalOptions.SlaveId);
        rootCommand.AddGlobalOption(globalOptions.Timeout);
        rootCommand.AddGlobalOption(globalOptions.Protocol);
        rootCommand.AddGlobalOption(globalOptions.SerialPort);
        rootCommand.AddGlobalOption(globalOptions.SerialBaud);
        rootCommand.AddGlobalOption(globalOptions.SerialDataBits);
        rootCommand.AddGlobalOption(globalOptions.SerialParity);
        rootCommand.AddGlobalOption(globalOptions.SerialStopBits);
        rootCommand.AddGlobalOption(globalOptions.VerbosityLevel);

        FlatCommandMode.ConfigureRoot(rootCommand, globalOptions);

        var normalizedArgs = ArgumentNormalizer.Normalize(args);
        return await rootCommand.InvokeAsync(normalizedArgs);
    }

    private static GlobalCliOptions CreateGlobalOptions()
    {
        var protocolOption = new Option<string?>(new[] { "--protocol", "-P" }, "Transport protocol: tcp, rtu-tcp, rtu-serial");
        var hostOption = new Option<string?>(new[] { "--host", "-H" }, "Modbus host (for TCP/RTU over TCP)");
        var targetOption = new Option<string?>(new[] { "--target", "-T" }, "Endpoint target. Examples: 192.168.1.10, 192.168.1.10:502, /dev/ttyUSB0, COM3");

        var portOption = new Option<int>(new[] { "--port", "-p" }, () => 502, "Modbus TCP port");
        var slaveIdOption = new Option<byte>(new[] { "--slave", "-s" }, () => 1, "Modbus slave/unit ID");
        var timeoutOption = new Option<int>(new[] { "--timeout", "-t" }, () => 5000, "Connection timeout in milliseconds");

        var serialPortOption = new Option<string?>(new[] { "--serial-port", "--serial" }, "Serial device path or port name");
        var serialBaudOption = new Option<int>("--baud", () => 9600, "Serial baud rate");
        var serialDataBitsOption = new Option<int>("--data-bits", () => 8, "Serial data bits");
        var serialParityOption = new Option<string>("--parity", () => "none", "Serial parity: none, even, odd");
        var serialStopBitsOption = new Option<string>("--stop-bits", () => "one", "Serial stop bits: one, two");

        var verbosityOption = new Option<int>("--verbosity-level", () => 0, "Verbosity level (0-3)")
        {
            IsHidden = true
        };

        return new GlobalCliOptions
        {
            Target = targetOption,
            Host = hostOption,
            Port = portOption,
            SlaveId = slaveIdOption,
            Timeout = timeoutOption,
            Protocol = protocolOption,
            SerialPort = serialPortOption,
            SerialBaud = serialBaudOption,
            SerialDataBits = serialDataBitsOption,
            SerialParity = serialParityOption,
            SerialStopBits = serialStopBitsOption,
            VerbosityLevel = verbosityOption
        };
    }
}
