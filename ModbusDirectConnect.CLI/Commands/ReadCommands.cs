using System.CommandLine;
using System.CommandLine.Invocation;
using ModbusDirectConnect.CLI.Client;
using ModbusDirectConnect.CLI.Infrastructure;
using ModbusDirectConnect.CLI.Output;
using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Commands;

public static class ReadCommands
{
    public static Command CreateReadCommand(GlobalCliOptions global)
    {
        var readCommand = new Command("read", "Read data from Modbus devices");

        var watchOption = new Option<bool>(new[] { "--watch", "-w" }, "Continuously read values until interrupted");
        var intervalOption = new Option<int>("--interval-ms", () => 1000, "Watch interval in milliseconds");
        var sameLineOption = new Option<bool>("--same-line", "Render watch output on one line");
        var noHeaderOption = new Option<bool>("--no-header", "Hide section headers");

        readCommand.AddOption(watchOption);
        readCommand.AddOption(intervalOption);
        readCommand.AddOption(sameLineOption);
        readCommand.AddOption(noHeaderOption);

        readCommand.AddCommand(CreateBooleanReadCommand(
            name: "coils",
            description: "Read coils (function code 1)",
            readOperation: (client, address, count) => client.ReadCoilsAsync(address, count),
            global,
            watchOption,
            intervalOption,
            sameLineOption,
            noHeaderOption));

        readCommand.AddCommand(CreateBooleanReadCommand(
            name: "discrete-inputs",
            description: "Read discrete inputs (function code 2)",
            readOperation: (client, address, count) => client.ReadDiscreteInputsAsync(address, count),
            global,
            watchOption,
            intervalOption,
            sameLineOption,
            noHeaderOption));

        readCommand.AddCommand(CreateRegisterReadCommand(
            name: "holding-registers",
            description: "Read holding registers (function code 3)",
            readOperation: (client, address, count) => client.ReadHoldingRegistersAsync(address, count),
            global,
            watchOption,
            intervalOption,
            sameLineOption,
            noHeaderOption));

        readCommand.AddCommand(CreateRegisterReadCommand(
            name: "input-registers",
            description: "Read input registers (function code 4)",
            readOperation: (client, address, count) => client.ReadInputRegistersAsync(address, count),
            global,
            watchOption,
            intervalOption,
            sameLineOption,
            noHeaderOption));

        return readCommand;
    }

    private static Command CreateBooleanReadCommand(
        string name,
        string description,
        Func<IModbusClient, ushort, ushort, Task<bool[]>> readOperation,
        GlobalCliOptions global,
        Option<bool> watchOption,
        Option<int> intervalOption,
        Option<bool> sameLineOption,
        Option<bool> noHeaderOption)
    {
        var command = new Command(name, description);
        var addressArgument = new Argument<ushort>("address", "Starting address");
        var countArgument = new Argument<ushort>("count", "Number of items to read");

        command.AddArgument(addressArgument);
        command.AddArgument(countArgument);

        command.SetHandler(async context =>
        {
            await ExecuteBooleanReadAsync(
                readOperation,
                BuildConnectionOptions(context, global),
                context.ParseResult.GetValueForOption(global.VerbosityLevel),
                context.ParseResult.GetValueForOption(watchOption),
                context.ParseResult.GetValueForOption(intervalOption),
                context.ParseResult.GetValueForOption(sameLineOption),
                context.ParseResult.GetValueForOption(noHeaderOption),
                context.ParseResult.GetValueForArgument(addressArgument),
                context.ParseResult.GetValueForArgument(countArgument));
        });

        return command;
    }

    private static Command CreateRegisterReadCommand(
        string name,
        string description,
        Func<IModbusClient, ushort, ushort, Task<ushort[]>> readOperation,
        GlobalCliOptions global,
        Option<bool> watchOption,
        Option<int> intervalOption,
        Option<bool> sameLineOption,
        Option<bool> noHeaderOption)
    {
        var command = new Command(name, description);
        var addressArgument = new Argument<ushort>("address", "Starting address");
        var countArgument = new Argument<ushort>("count", "Number of registers to read");

        var formatOption = new Option<string>(new[] { "--format", "-f" }, () => "u16", "Register output format: u16, hex, ascii, utf8, cstring");
        var wordOrderOption = new Option<string>("--word-order", () => "be", "Byte order per register: be or le");

        command.AddArgument(addressArgument);
        command.AddArgument(countArgument);
        command.AddOption(formatOption);
        command.AddOption(wordOrderOption);

        command.SetHandler(async context =>
        {
            await ExecuteRegisterReadAsync(
                readOperation,
                BuildConnectionOptions(context, global),
                context.ParseResult.GetValueForOption(global.VerbosityLevel),
                context.ParseResult.GetValueForOption(watchOption),
                context.ParseResult.GetValueForOption(intervalOption),
                context.ParseResult.GetValueForOption(sameLineOption),
                context.ParseResult.GetValueForOption(noHeaderOption),
                context.ParseResult.GetValueForOption(formatOption)!,
                context.ParseResult.GetValueForOption(wordOrderOption)!,
                context.ParseResult.GetValueForArgument(addressArgument),
                context.ParseResult.GetValueForArgument(countArgument));
        });

        return command;
    }

    private static async Task ExecuteBooleanReadAsync(
        Func<IModbusClient, ushort, ushort, Task<bool[]>> readOperation,
        ConnectionOptions connectionOptions,
        int verbosityLevel,
        bool watch,
        int intervalMs,
        bool sameLine,
        bool noHeader,
        ushort address,
        ushort count)
    {
        try
        {
            var logger = new VerbosityLogger(verbosityLevel);
            var connection = EndpointResolver.Resolve(connectionOptions);

            logger.Log(1, $"Reading {count} values from {address} ({connection.Transport})");
            logger.Log(2, $"Endpoint: {connection.DisplayTarget}, slave={connection.SlaveId}, timeout={connection.Timeout}ms");
            logger.Log(3, $"Transport settings: baud={connection.SerialBaud}, dataBits={connection.SerialDataBits}, parity={connection.SerialParity}, stopBits={connection.SerialStopBits}");

            using var client = ModbusClientFactory.CreateClient(connection);

            if (watch)
            {
                await WatchAsync(async () =>
                {
                    var values = await readOperation(client, address, count);
                    if (sameLine)
                    {
                        var line = $"{DateTimeOffset.Now:HH:mm:ss} {string.Join(' ', values.Select(v => v ? '1' : '0'))}";
                        RewriteLine(line);
                        return;
                    }

                    PrintBooleanResults(address, values, noHeader);
                }, intervalMs);

                return;
            }

            var result = await readOperation(client, address, count);
            PrintBooleanResults(address, result, noHeader);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading values: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task ExecuteRegisterReadAsync(
        Func<IModbusClient, ushort, ushort, Task<ushort[]>> readOperation,
        ConnectionOptions connectionOptions,
        int verbosityLevel,
        bool watch,
        int intervalMs,
        bool sameLine,
        bool noHeader,
        string format,
        string wordOrder,
        ushort address,
        ushort count)
    {
        try
        {
            var parsedFormat = RegisterFormatter.ParseFormat(format);
            var isBigEndian = ParseBigEndian(wordOrder);

            var logger = new VerbosityLogger(verbosityLevel);
            var connection = EndpointResolver.Resolve(connectionOptions);

            logger.Log(1, $"Reading {count} registers from {address} ({connection.Transport})");
            logger.Log(2, $"Endpoint: {connection.DisplayTarget}, slave={connection.SlaveId}, timeout={connection.Timeout}ms");
            logger.Log(3, $"Transport settings: baud={connection.SerialBaud}, dataBits={connection.SerialDataBits}, parity={connection.SerialParity}, stopBits={connection.SerialStopBits}");

            using var client = ModbusClientFactory.CreateClient(connection);

            if (watch)
            {
                await WatchAsync(async () =>
                {
                    var values = await readOperation(client, address, count);
                    if (sameLine)
                    {
                        var text = RegisterFormatter.FormatSingleLine(values, parsedFormat, isBigEndian);
                        RewriteLine($"{DateTimeOffset.Now:HH:mm:ss} {text}");
                        return;
                    }

                    PrintRegisterResults(address, values, parsedFormat, isBigEndian, noHeader);
                }, intervalMs);

                return;
            }

            var result = await readOperation(client, address, count);
            PrintRegisterResults(address, result, parsedFormat, isBigEndian, noHeader);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading registers: {ex.Message}");
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

    private static async Task WatchAsync(Func<Task> readAction, int intervalMs)
    {
        if (intervalMs <= 0)
        {
            throw new ArgumentException("--interval-ms must be greater than 0");
        }

        var cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            await readAction();

            try
            {
                await Task.Delay(intervalMs, cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        Console.WriteLine();
    }

    private static void PrintBooleanResults(ushort startAddress, bool[] results, bool noHeader)
    {
        if (!noHeader)
        {
            Console.WriteLine($"{DateTimeOffset.Now:O}");
        }

        for (var i = 0; i < results.Length; i++)
        {
            Console.WriteLine($"{startAddress + i}: {results[i]}");
        }

        if (!noHeader)
        {
            Console.WriteLine();
        }
    }

    private static void PrintRegisterResults(ushort startAddress, ushort[] results, RegisterFormat format, bool bigEndian, bool noHeader)
    {
        if (!noHeader)
        {
            Console.WriteLine($"{DateTimeOffset.Now:O}");
        }

        var lines = RegisterFormatter.FormatRegisters(startAddress, results, format, bigEndian);
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }

        if (!noHeader)
        {
            Console.WriteLine();
        }
    }

    private static bool ParseBigEndian(string wordOrder)
    {
        return wordOrder.Trim().ToLowerInvariant() switch
        {
            "be" => true,
            "le" => false,
            _ => throw new ArgumentException("--word-order must be either 'be' or 'le'")
        };
    }

    private static void RewriteLine(string text)
    {
        Console.Write("\r");
        Console.Write(text.PadRight(Console.WindowWidth > 0 ? Console.WindowWidth - 1 : text.Length));
    }
}
