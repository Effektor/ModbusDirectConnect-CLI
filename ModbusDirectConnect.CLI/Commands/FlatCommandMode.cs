using System.CommandLine;
using System.CommandLine.Invocation;
using ModbusDirectConnect.CLI.Client;
using ModbusDirectConnect.CLI.Infrastructure;
using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Commands;

public static class FlatCommandMode
{
    public static void ConfigureRoot(RootCommand rootCommand, GlobalCliOptions global)
    {
        var readCoilOption = new Option<ushort?>(new[] { "--read-coil", "--rc", "-rc" }, "Read coils starting at address");
        var readDiscreteOption = new Option<ushort?>(new[] { "--read-discrete", "--rd", "-rd" }, "Read discrete inputs starting at address");
        var readHoldingOption = new Option<ushort?>(new[] { "--read-holding", "--rh", "-rh" }, "Read holding registers starting at address");
        var readInputRegOption = new Option<ushort?>(new[] { "--read-inputreg", "--ri", "-ri" }, "Read input registers starting at address");

        var writeCoilOption = new Option<ushort?>(new[] { "--write-coil", "--wc", "-wc" }, "Write single coil at address");
        var writeRegOption = new Option<ushort?>(new[] { "--write-reg", "--wr", "-wr" }, "Write single register at address");
        var writeMultiCoilOption = new Option<ushort?>("--write-multi-coil", "Write multiple coils starting at address");
        var writeMultiRegOption = new Option<ushort?>("--write-multi-reg", "Write multiple registers starting at address");

        var countOption = new Option<ushort>(new[] { "--count", "-c" }, () => 1, "Number of values to read");
        var dataOption = new Option<string?>(new[] { "--data", "-d" }, "Write data payload (hex or decimal, comma-separated for multiple)");

        var rtuOption = new Option<bool>("--rtu", "Use RTU framing. With TCP target this means RTU-over-TCP.");
        var asciiOption = new Option<bool>("--ascii", "Use Modbus ASCII framing (serial only; currently not implemented).");
        var serialOption = new Option<string?>("--serial", "Serial port path, e.g. COM5 or /dev/serial1");

        rootCommand.AddOption(readCoilOption);
        rootCommand.AddOption(readDiscreteOption);
        rootCommand.AddOption(readHoldingOption);
        rootCommand.AddOption(readInputRegOption);
        rootCommand.AddOption(writeCoilOption);
        rootCommand.AddOption(writeRegOption);
        rootCommand.AddOption(writeMultiCoilOption);
        rootCommand.AddOption(writeMultiRegOption);
        rootCommand.AddOption(countOption);
        rootCommand.AddOption(dataOption);
        rootCommand.AddOption(rtuOption);
        rootCommand.AddOption(asciiOption);
        rootCommand.AddOption(serialOption);

        rootCommand.SetHandler(async context =>
        {
            var selectedOperations = new List<string>();
            if (context.ParseResult.GetValueForOption(readCoilOption).HasValue) selectedOperations.Add("read-coil");
            if (context.ParseResult.GetValueForOption(readDiscreteOption).HasValue) selectedOperations.Add("read-discrete");
            if (context.ParseResult.GetValueForOption(readHoldingOption).HasValue) selectedOperations.Add("read-holding");
            if (context.ParseResult.GetValueForOption(readInputRegOption).HasValue) selectedOperations.Add("read-inputreg");
            if (context.ParseResult.GetValueForOption(writeCoilOption).HasValue) selectedOperations.Add("write-coil");
            if (context.ParseResult.GetValueForOption(writeRegOption).HasValue) selectedOperations.Add("write-reg");
            if (context.ParseResult.GetValueForOption(writeMultiCoilOption).HasValue) selectedOperations.Add("write-multi-coil");
            if (context.ParseResult.GetValueForOption(writeMultiRegOption).HasValue) selectedOperations.Add("write-multi-reg");

            if (selectedOperations.Count == 0)
            {
                context.Console.Error.Write("No operation selected. Use --read-coil/--read-holding/--write-reg/etc.\n");
                context.ExitCode = 1;
                return;
            }

            if (selectedOperations.Count > 1)
            {
                context.Console.Error.Write("Specify exactly one operation flag at a time.\n");
                context.ExitCode = 1;
                return;
            }

            try
            {
                var verbosity = context.ParseResult.GetValueForOption(global.VerbosityLevel);
                var logger = new VerbosityLogger(verbosity);

                var explicitSerial = context.ParseResult.GetValueForOption(serialOption)
                    ?? context.ParseResult.GetValueForOption(global.SerialPort);
                var target = context.ParseResult.GetValueForOption(global.Target);
                var protocol = context.ParseResult.GetValueForOption(global.Protocol);

                if (!string.IsNullOrWhiteSpace(explicitSerial))
                {
                    target ??= explicitSerial;
                    protocol ??= "rtu-serial";
                }

                if (context.ParseResult.GetValueForOption(rtuOption) && string.IsNullOrWhiteSpace(protocol))
                {
                    protocol = IsSerialLike(target) ? "rtu-serial" : "rtu-tcp";
                }

                if (context.ParseResult.GetValueForOption(asciiOption))
                {
                    throw new NotSupportedException("Modbus ASCII transport mode is not implemented yet.");
                }

                var connectionOptions = new ConnectionOptions(
                    Target: target,
                    Host: context.ParseResult.GetValueForOption(global.Host),
                    Port: context.ParseResult.GetValueForOption(global.Port),
                    SlaveId: context.ParseResult.GetValueForOption(global.SlaveId),
                    Timeout: context.ParseResult.GetValueForOption(global.Timeout),
                    Protocol: protocol,
                    SerialPort: explicitSerial,
                    SerialBaud: context.ParseResult.GetValueForOption(global.SerialBaud),
                    SerialDataBits: context.ParseResult.GetValueForOption(global.SerialDataBits),
                    SerialParity: context.ParseResult.GetValueForOption(global.SerialParity)!,
                    SerialStopBits: context.ParseResult.GetValueForOption(global.SerialStopBits)!);

                var connection = EndpointResolver.Resolve(connectionOptions);
                logger.Log(1, $"Operation over {connection.Transport}");
                logger.Log(2, $"Endpoint: {connection.DisplayTarget}, slave={connection.SlaveId}, timeout={connection.Timeout}ms");
                logger.Log(3, $"Transport settings: baud={connection.SerialBaud}, dataBits={connection.SerialDataBits}, parity={connection.SerialParity}, stopBits={connection.SerialStopBits}");

                using var client = ModbusClientFactory.CreateClient(connection);

                var count = context.ParseResult.GetValueForOption(countOption);
                var data = context.ParseResult.GetValueForOption(dataOption);

                var readCoilAddress = context.ParseResult.GetValueForOption(readCoilOption);
                if (readCoilAddress.HasValue)
                {
                    var result = await client.ReadCoilsAsync(readCoilAddress.Value, count);
                    PrintBooleanResults(readCoilAddress.Value, result);
                    return;
                }

                var readDiscreteAddress = context.ParseResult.GetValueForOption(readDiscreteOption);
                if (readDiscreteAddress.HasValue)
                {
                    var result = await client.ReadDiscreteInputsAsync(readDiscreteAddress.Value, count);
                    PrintBooleanResults(readDiscreteAddress.Value, result);
                    return;
                }

                var readHoldingAddress = context.ParseResult.GetValueForOption(readHoldingOption);
                if (readHoldingAddress.HasValue)
                {
                    var result = await client.ReadHoldingRegistersAsync(readHoldingAddress.Value, count);
                    PrintRegisterResults(readHoldingAddress.Value, result);
                    return;
                }

                var readInputAddress = context.ParseResult.GetValueForOption(readInputRegOption);
                if (readInputAddress.HasValue)
                {
                    var result = await client.ReadInputRegistersAsync(readInputAddress.Value, count);
                    PrintRegisterResults(readInputAddress.Value, result);
                    return;
                }

                var writeCoilAddress = context.ParseResult.GetValueForOption(writeCoilOption);
                if (writeCoilAddress.HasValue)
                {
                    var value = ParseCoilWriteData(data);
                    await client.WriteSingleCoilAsync(writeCoilAddress.Value, value);
                    Console.WriteLine($"Wrote coil {writeCoilAddress.Value}: {value}");
                    return;
                }

                var writeRegAddress = context.ParseResult.GetValueForOption(writeRegOption);
                if (writeRegAddress.HasValue)
                {
                    var value = ParseRegisterWriteData(data);
                    await client.WriteSingleRegisterAsync(writeRegAddress.Value, value);
                    Console.WriteLine($"Wrote register {writeRegAddress.Value}: {value}");
                    return;
                }

                var writeMultiCoilAddress = context.ParseResult.GetValueForOption(writeMultiCoilOption);
                if (writeMultiCoilAddress.HasValue)
                {
                    var values = ParseMultiCoilData(data);
                    await client.WriteMultipleCoilsAsync(writeMultiCoilAddress.Value, values);
                    Console.WriteLine($"Wrote {values.Length} coils starting at {writeMultiCoilAddress.Value}");
                    return;
                }

                var writeMultiRegAddress = context.ParseResult.GetValueForOption(writeMultiRegOption);
                if (writeMultiRegAddress.HasValue)
                {
                    var values = ParseMultiRegisterData(data);
                    await client.WriteMultipleRegistersAsync(writeMultiRegAddress.Value, values);
                    Console.WriteLine($"Wrote {values.Length} registers starting at {writeMultiRegAddress.Value}");
                    return;
                }
            }
            catch (Exception ex)
            {
                context.Console.Error.Write($"Error: {ex.Message}\n");
                context.ExitCode = 1;
            }
        });
    }

    private static bool ParseCoilWriteData(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data is required for write operations");
        }

        if (bool.TryParse(data, out var boolValue))
        {
            return boolValue;
        }

        var value = ParseNumericWord(data);
        return value != 0;
    }

    private static ushort ParseRegisterWriteData(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data is required for write operations");
        }

        return ParseNumericWord(data);
    }

    private static bool[] ParseMultiCoilData(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data is required for write operations");
        }

        if (data.Contains(',', StringComparison.Ordinal))
        {
            return data
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(ParseCoilWriteData)
                .ToArray();
        }

        if (data.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            var value = ParseNumericWord(data);
            return Enumerable.Range(0, 16)
                .Select(bit => ((value >> bit) & 0x1) == 1)
                .ToArray();
        }

        if (data.All(char.IsDigit))
        {
            return data.Select(ch => ch != '0').ToArray();
        }

        throw new ArgumentException("Invalid --data for --write-multi-coil. Use comma-separated booleans, digit sequence, or hex word (0x....)");
    }

    private static ushort[] ParseMultiRegisterData(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data is required for write operations");
        }

        if (!data.Contains(',', StringComparison.Ordinal))
        {
            return new[] { ParseNumericWord(data) };
        }

        return data
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParseNumericWord)
            .ToArray();
    }

    private static ushort ParseNumericWord(string input)
    {
        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return Convert.ToUInt16(input[2..], 16);
        }

        if (ushort.TryParse(input, out var value))
        {
            return value;
        }

        throw new ArgumentException($"Invalid numeric data value: {input}");
    }

    private static bool IsSerialLike(string? target)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            return false;
        }

        return target.StartsWith("/dev/", StringComparison.OrdinalIgnoreCase)
            || target.StartsWith("com", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintBooleanResults(ushort startAddress, bool[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"{startAddress + i}: {values[i]}");
        }
    }

    private static void PrintRegisterResults(ushort startAddress, ushort[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"{startAddress + i}: {values[i]}");
        }
    }
}
