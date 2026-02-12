using System.CommandLine;
using System.CommandLine.Invocation;
using ModbusDirectConnect.CLI.Client;
using ModbusDirectConnect.CLI.Infrastructure;
using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Commands;

public static class WriteCommands
{
    public static Command CreateWriteCommand(GlobalCliOptions global)
    {
        var writeCommand = new Command("write", "Write data to Modbus devices");

        writeCommand.AddCommand(CreateWriteSingleCoilCommand(global));
        writeCommand.AddCommand(CreateWriteSingleRegisterCommand(global));
        writeCommand.AddCommand(CreateWriteMultipleCoilsCommand(global));
        writeCommand.AddCommand(CreateWriteMultipleRegistersCommand(global));

        return writeCommand;
    }

    private static Command CreateWriteSingleCoilCommand(GlobalCliOptions global)
    {
        var command = new Command("coil", "Write single coil (function code 5)");
        var addressArgument = new Argument<ushort>("address", "Address to write");
        var valueArgument = new Argument<bool>("value", "Value to write (true/false or 1/0)");

        command.AddArgument(addressArgument);
        command.AddArgument(valueArgument);

        command.SetHandler(async context =>
        {
            var address = context.ParseResult.GetValueForArgument(addressArgument);
            var value = context.ParseResult.GetValueForArgument(valueArgument);

            await ExecuteWriteAsync(
                BuildConnectionOptions(context, global),
                context.ParseResult.GetValueForOption(global.VerbosityLevel),
                async client => await client.WriteSingleCoilAsync(address, value),
                $"Wrote coil {address}: {value}");
        });

        return command;
    }

    private static Command CreateWriteSingleRegisterCommand(GlobalCliOptions global)
    {
        var command = new Command("register", "Write single holding register (function code 6)");
        var addressArgument = new Argument<ushort>("address", "Address to write");
        var valueArgument = new Argument<ushort>("value", "Value to write");

        command.AddArgument(addressArgument);
        command.AddArgument(valueArgument);

        command.SetHandler(async context =>
        {
            var address = context.ParseResult.GetValueForArgument(addressArgument);
            var value = context.ParseResult.GetValueForArgument(valueArgument);

            await ExecuteWriteAsync(
                BuildConnectionOptions(context, global),
                context.ParseResult.GetValueForOption(global.VerbosityLevel),
                async client => await client.WriteSingleRegisterAsync(address, value),
                $"Wrote register {address}: {value}");
        });

        return command;
    }

    private static Command CreateWriteMultipleCoilsCommand(GlobalCliOptions global)
    {
        var command = new Command("coils", "Write multiple coils (function code 15)");
        var addressArgument = new Argument<ushort>("address", "Starting address");
        var valuesArgument = new Argument<string>("values", "Comma-separated boolean values (e.g., true,false,true or 1,0,1)");

        command.AddArgument(addressArgument);
        command.AddArgument(valuesArgument);

        command.SetHandler(async context =>
        {
            var address = context.ParseResult.GetValueForArgument(addressArgument);
            var values = context.ParseResult.GetValueForArgument(valuesArgument);
            var parsed = ParseBooleanArray(values);

            await ExecuteWriteAsync(
                BuildConnectionOptions(context, global),
                context.ParseResult.GetValueForOption(global.VerbosityLevel),
                async client => await client.WriteMultipleCoilsAsync(address, parsed),
                $"Wrote {parsed.Length} coils starting at {address}");
        });

        return command;
    }

    private static Command CreateWriteMultipleRegistersCommand(GlobalCliOptions global)
    {
        var command = new Command("registers", "Write multiple holding registers (function code 16)");
        var addressArgument = new Argument<ushort>("address", "Starting address");
        var valuesArgument = new Argument<string>("values", "Comma-separated register values (e.g., 100,200,300)");

        command.AddArgument(addressArgument);
        command.AddArgument(valuesArgument);

        command.SetHandler(async context =>
        {
            var address = context.ParseResult.GetValueForArgument(addressArgument);
            var values = context.ParseResult.GetValueForArgument(valuesArgument);
            var parsed = ParseUshortArray(values);

            await ExecuteWriteAsync(
                BuildConnectionOptions(context, global),
                context.ParseResult.GetValueForOption(global.VerbosityLevel),
                async client => await client.WriteMultipleRegistersAsync(address, parsed),
                $"Wrote {parsed.Length} registers starting at {address}");
        });

        return command;
    }

    private static async Task ExecuteWriteAsync(
        ConnectionOptions connectionOptions,
        int verbosity,
        Func<IModbusClient, Task> writeOperation,
        string successMessage)
    {
        try
        {
            var logger = new VerbosityLogger(verbosity);
            var connection = EndpointResolver.Resolve(connectionOptions);

            logger.Log(1, $"Write using {connection.Transport}");
            logger.Log(2, $"Endpoint: {connection.DisplayTarget}, slave={connection.SlaveId}, timeout={connection.Timeout}ms");
            logger.Log(3, $"Transport settings: baud={connection.SerialBaud}, dataBits={connection.SerialDataBits}, parity={connection.SerialParity}, stopBits={connection.SerialStopBits}");

            using var client = ModbusClientFactory.CreateClient(connection);
            await writeOperation(client);
            Console.WriteLine(successMessage);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error writing values: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static ConnectionOptions BuildConnectionOptions(InvocationContext context, GlobalCliOptions global)
    {
        return new ConnectionOptions(
            Target: context.ParseResult.GetValueForOption(global.Target),
            Host: context.ParseResult.GetValueForOption(global.Host),
            Port: context.ParseResult.GetValueForOption(global.Port),
            SlaveId: context.ParseResult.GetValueForOption(global.SlaveId),
            Timeout: context.ParseResult.GetValueForOption(global.Timeout),
            Protocol: context.ParseResult.GetValueForOption(global.Protocol),
            SerialPort: context.ParseResult.GetValueForOption(global.SerialPort),
            SerialBaud: context.ParseResult.GetValueForOption(global.SerialBaud),
            SerialDataBits: context.ParseResult.GetValueForOption(global.SerialDataBits),
            SerialParity: context.ParseResult.GetValueForOption(global.SerialParity)!,
            SerialStopBits: context.ParseResult.GetValueForOption(global.SerialStopBits)!);
    }

    private static bool[] ParseBooleanArray(string valuesStr)
    {
        var parts = valuesStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new bool[parts.Length];

        for (var i = 0; i < parts.Length; i++)
        {
            if (bool.TryParse(parts[i], out var boolValue))
            {
                result[i] = boolValue;
                continue;
            }

            if (int.TryParse(parts[i], out var intValue))
            {
                result[i] = intValue != 0;
                continue;
            }

            throw new ArgumentException($"Invalid boolean value: {parts[i]}");
        }

        return result;
    }

    private static ushort[] ParseUshortArray(string valuesStr)
    {
        var parts = valuesStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new ushort[parts.Length];

        for (var i = 0; i < parts.Length; i++)
        {
            if (!ushort.TryParse(parts[i], out result[i]))
            {
                throw new ArgumentException($"Invalid register value: {parts[i]}");
            }
        }

        return result;
    }
}
