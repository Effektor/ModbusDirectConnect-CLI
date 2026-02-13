using System.CommandLine;
using ModbusDirectConnect.CLI.Commands;
using ModbusDirectConnect.CLI.Infrastructure;
using System.Reflection;

namespace ModbusDirectConnect.CLI;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var normalizedArgs = ArgumentNormalizer.Normalize(args);

        if (normalizedArgs.Length == 0 || normalizedArgs.Any(a => a is "-h" or "--help"))
        {
            Console.WriteLine(HelpTextProvider.GetHelpText());
            return 0;
        }

        if (normalizedArgs.Any(a => a == "--version"))
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            Console.WriteLine(version);
            return 0;
        }

        var rootCommand = BuildRootCommand();
        return await rootCommand.InvokeAsync(normalizedArgs);
    }

    internal static RootCommand BuildRootCommand()
    {
        var rootCommand = new RootCommand("mbdc - Modbus debug client (TCP / RTU / ASCII) with decoding and monitoring")
        {
            Description = "Use --help to show full technical help from HELP.md."
        };
        var globalOptions = CreateGlobalOptions();

        rootCommand.AddGlobalOption(globalOptions.Target);
        rootCommand.AddGlobalOption(globalOptions.Host);
        rootCommand.AddGlobalOption(globalOptions.Port);
        rootCommand.AddGlobalOption(globalOptions.SlaveId);
        rootCommand.AddGlobalOption(globalOptions.TimeoutSeconds);
        rootCommand.AddGlobalOption(globalOptions.Retries);
        rootCommand.AddGlobalOption(globalOptions.Protocol);
        rootCommand.AddGlobalOption(globalOptions.SerialPort);
        rootCommand.AddGlobalOption(globalOptions.SerialBaud);
        rootCommand.AddGlobalOption(globalOptions.SerialDataBits);
        rootCommand.AddGlobalOption(globalOptions.SerialParity);
        rootCommand.AddGlobalOption(globalOptions.SerialStopBits);
        rootCommand.AddGlobalOption(globalOptions.VerbosityLevel);

        FlatCommandMode.ConfigureRoot(rootCommand, globalOptions);
        return rootCommand;
    }

    private static GlobalCliOptions CreateGlobalOptions()
    {
        var protocolOption = new Option<string?>(new[] { "--protocol", "-P" }, "Transport protocol: tcp, rtu-tcp, rtu-serial");
        var hostOption = new Option<string?>(new[] { "--host", "-H" }, "Modbus host (for TCP/RTU-over-TCP)");
        var targetOption = new Option<string?>(new[] { "--target", "-T" }, "Endpoint target. Examples: 192.168.1.10, 192.168.1.10:502, /dev/ttyUSB0, COM3");

        var portOption = new Option<int>(new[] { "--port", "-p" }, () => 502, "Modbus TCP port");
        var slaveIdOption = new Option<byte>(new[] { "--unit", "-u", "--slave", "-s" }, () => 1, "Unit identifier / slave address");
        var timeoutOption = new Option<double>(new[] { "--timeout", "-t" }, () => 1.0, "Response timeout in seconds");
        var retriesOption = new Option<int>("--retries", () => 0, "Retries on timeout");

        var serialPortOption = new Option<string?>(new[] { "--serial-port", "--serial" }, "Serial device path or port name");
        var serialBaudOption = new Option<int>("--baud", () => 19200, "Serial baud rate");
        var serialDataBitsOption = new Option<int>(new[] { "--databits", "--data-bits" }, () => 8, "Serial data bits (7|8)");
        var serialParityOption = new Option<string>("--parity", () => "N", "Serial parity (N|E|O)");
        var serialStopBitsOption = new Option<string>(new[] { "--stopbits", "--stop-bits" }, () => "1", "Serial stop bits (1|2)");

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
            TimeoutSeconds = timeoutOption,
            Retries = retriesOption,
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
