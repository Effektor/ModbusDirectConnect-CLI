using System.CommandLine;
using ModbusDirectConnect.CLI.Commands;

namespace ModbusDirectConnect.CLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("ModbusDirectConnect-CLI - A command-line tool for Modbus communication")
        {
            Description = "Cross-platform CLI tool for reading and writing Modbus addresses using TCP or RTU protocols."
        };

        // Add global options
        var hostOption = new Option<string>(
            aliases: new[] { "--host", "-H" },
            getDefaultValue: () => "localhost",
            description: "Modbus host address (for TCP)");

        var portOption = new Option<int>(
            aliases: new[] { "--port", "-p" },
            getDefaultValue: () => 502,
            description: "Modbus port (for TCP)");

        var slaveIdOption = new Option<byte>(
            aliases: new[] { "--slave", "-s" },
            getDefaultValue: () => 1,
            description: "Modbus slave/unit ID");

        var timeoutOption = new Option<int>(
            aliases: new[] { "--timeout", "-t" },
            getDefaultValue: () => 5000,
            description: "Connection timeout in milliseconds");

        var protocolOption = new Option<string>(
            aliases: new[] { "--protocol" },
            getDefaultValue: () => "tcp",
            description: "Protocol type (tcp or rtu)");

        rootCommand.AddGlobalOption(hostOption);
        rootCommand.AddGlobalOption(portOption);
        rootCommand.AddGlobalOption(slaveIdOption);
        rootCommand.AddGlobalOption(timeoutOption);
        rootCommand.AddGlobalOption(protocolOption);

        // Add commands
        rootCommand.AddCommand(ReadCommands.CreateReadCommand(hostOption, portOption, slaveIdOption, timeoutOption, protocolOption));
        rootCommand.AddCommand(WriteCommands.CreateWriteCommand(hostOption, portOption, slaveIdOption, timeoutOption, protocolOption));

        return await rootCommand.InvokeAsync(args);
    }
}
