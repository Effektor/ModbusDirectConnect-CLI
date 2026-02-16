using System.CommandLine;
using System.CommandLine.Invocation;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ModbusDirectConnect.CLI.Client;
using ModbusDirectConnect.CLI.Infrastructure;
using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Commands;

public static class FlatCommandMode
{
    public static void ConfigureRoot(RootCommand rootCommand, GlobalCliOptions global)
    {
        var readCoilOption = new Option<string?>(new[] { "--read-coil", "-rc", "--rc" }, "Read coils (FC01)");
        var readDiscreteOption = new Option<string?>(new[] { "--read-discrete", "-rd", "--rd" }, "Read discrete inputs (FC02)");
        var readHoldingOption = new Option<string?>(new[] { "--read-holding", "-rh", "--rh" }, "Read holding registers (FC03)");
        var readInputRegOption = new Option<string?>(new[] { "--read-inputreg", "-ri", "--ri" }, "Read input registers (FC04)");
        var readRefOption = new Option<string?>("--ref", "Read using Modicon reference addressing");

        var writeCoilOption = new Option<string?>(new[] { "--write-coil", "-wc", "--wc" }, "Write single coil (FC05)");
        var writeRegOption = new Option<string?>(new[] { "--write-reg", "-wr", "--wr" }, "Write single register (FC06)");
        var writeMultiCoilOption = new Option<string?>(new[] { "--write-multi-coil", "-wmc", "--wmc" }, "Write multiple coils (FC15)");
        var writeMultiRegOption = new Option<string?>(new[] { "--write-multi-reg", "-wmr", "--wmr" }, "Write multiple registers (FC16)");

        var countOption = new Option<ushort>(new[] { "--count", "-c" }, () => 1, "Number of items to read");
        var dataOption = new Option<string?>(new[] { "--data", "-d" }, "Write payload");
        var valueOption = new Option<string?>("--value", "Alias of --data");
        var onOption = new Option<bool>("--on", "Convenience for writing coil ON");
        var offOption = new Option<bool>("--off", "Convenience for writing coil OFF");
        var raw16Option = new Option<string?>("--raw16", "Raw 16-bit payload");
        var rawBytesOption = new Option<string?>("--raw-bytes", "Raw byte payload");
        var bitsOption = CreateOptionalStringOption("--bits", "Bit string/CSV for multi-coil write, or bitstring output for reads");
        var bytesOption = CreateOptionalStringOption("--bytes", "Packed bytes for multi-coil write, or raw byte output for reads");

        var tcpOption = new Option<bool>("--tcp", "Force Modbus/TCP transport");
        var rtuOption = new Option<bool>("--rtu", "Use RTU framing");
        var asciiOption = new Option<bool>("--ascii", "Use Modbus ASCII framing");

        var u16Option = new Option<bool>("--u16", "Decode as unsigned 16-bit");
        var s16Option = new Option<bool>("--s16", "Decode as signed 16-bit");
        var u32Option = new Option<bool>("--u32", "Decode as unsigned 32-bit");
        var s32Option = new Option<bool>("--s32", "Decode as signed 32-bit");
        var f32Option = new Option<bool>("--f32", "Decode as float32");
        var f64Option = new Option<bool>("--f64", "Decode as float64");

        var bigEndianOption = new Option<bool>("--be", "Big-endian within word");
        var littleEndianOption = new Option<bool>(new[] { "--le", "--little-endian" }, "Little-endian within word");
        var wordSwapOption = new Option<bool>(new[] { "--ws", "--word-swap" }, "Swap word order");
        var byteSwapOption = new Option<bool>(new[] { "--bs", "--byte-swap" }, "Alias of --le");

        var stringOption = new Option<bool>("--string", "Decode registers as string bytes");
        var stringLenOption = new Option<int?>("--string-len", "Limit string decode length");
        var nullTermOption = new Option<bool>("--null-term", "Stop string decode at first NUL");
        var encodingOption = new Option<string>("--encoding", () => "utf-8", "String encoding for --string");

        var boolOption = new Option<bool>("--bool", "Print each read bit as true/false");
        var hexOption = new Option<bool>("--hex", "Print packed bytes as hex");

        var watchOption = CreateOptionalIntervalOption("--watch", "Continuously re-read and append output");
        var monitorOption = CreateOptionalIntervalOption("--monitor", "Continuously re-read and redraw output");
        var diffOption = new Option<bool>("--diff", "Highlight changed values");
        var onlyChangedOption = new Option<bool>("--only-changed", "Only show changed values");
        var groupOption = new Option<int?>("--group", "Group output values");
        var scanOption = new Option<string?>("--scan", "Scan mode type");

        var quietOption = new Option<bool>(new[] { "--quiet", "-q" }, "Minimal output");
        var jsonOption = new Option<bool>("--json", "JSON output");
        var noColorOption = new Option<bool>("--no-color", "Disable ANSI colors");
        var timestampOption = new Option<bool>("--timestamp", "Prefix output lines with timestamp");

        rootCommand.AddOption(readCoilOption);
        rootCommand.AddOption(readDiscreteOption);
        rootCommand.AddOption(readHoldingOption);
        rootCommand.AddOption(readInputRegOption);
        rootCommand.AddOption(readRefOption);

        rootCommand.AddOption(writeCoilOption);
        rootCommand.AddOption(writeRegOption);
        rootCommand.AddOption(writeMultiCoilOption);
        rootCommand.AddOption(writeMultiRegOption);

        rootCommand.AddOption(countOption);
        rootCommand.AddOption(dataOption);
        rootCommand.AddOption(valueOption);
        rootCommand.AddOption(onOption);
        rootCommand.AddOption(offOption);
        rootCommand.AddOption(raw16Option);
        rootCommand.AddOption(rawBytesOption);
        rootCommand.AddOption(bitsOption);
        rootCommand.AddOption(bytesOption);

        rootCommand.AddOption(tcpOption);
        rootCommand.AddOption(rtuOption);
        rootCommand.AddOption(asciiOption);

        rootCommand.AddOption(u16Option);
        rootCommand.AddOption(s16Option);
        rootCommand.AddOption(u32Option);
        rootCommand.AddOption(s32Option);
        rootCommand.AddOption(f32Option);
        rootCommand.AddOption(f64Option);
        rootCommand.AddOption(bigEndianOption);
        rootCommand.AddOption(littleEndianOption);
        rootCommand.AddOption(wordSwapOption);
        rootCommand.AddOption(byteSwapOption);
        rootCommand.AddOption(stringOption);
        rootCommand.AddOption(stringLenOption);
        rootCommand.AddOption(nullTermOption);
        rootCommand.AddOption(encodingOption);
        rootCommand.AddOption(boolOption);
        rootCommand.AddOption(hexOption);

        rootCommand.AddOption(watchOption);
        rootCommand.AddOption(monitorOption);
        rootCommand.AddOption(diffOption);
        rootCommand.AddOption(onlyChangedOption);
        rootCommand.AddOption(groupOption);
        rootCommand.AddOption(scanOption);

        rootCommand.AddOption(quietOption);
        rootCommand.AddOption(jsonOption);
        rootCommand.AddOption(noColorOption);
        rootCommand.AddOption(timestampOption);

        rootCommand.SetHandler(async context =>
        {
            var selectedOps = new List<string>();
            if (context.ParseResult.GetValueForOption(readCoilOption) is not null) selectedOps.Add("read-coil");
            if (context.ParseResult.GetValueForOption(readDiscreteOption) is not null) selectedOps.Add("read-discrete");
            if (context.ParseResult.GetValueForOption(readHoldingOption) is not null) selectedOps.Add("read-holding");
            if (context.ParseResult.GetValueForOption(readInputRegOption) is not null) selectedOps.Add("read-inputreg");
            if (context.ParseResult.GetValueForOption(readRefOption) is not null) selectedOps.Add("read-ref");
            if (context.ParseResult.GetValueForOption(writeCoilOption) is not null) selectedOps.Add("write-coil");
            if (context.ParseResult.GetValueForOption(writeRegOption) is not null) selectedOps.Add("write-reg");
            if (context.ParseResult.GetValueForOption(writeMultiCoilOption) is not null) selectedOps.Add("write-multi-coil");
            if (context.ParseResult.GetValueForOption(writeMultiRegOption) is not null) selectedOps.Add("write-multi-reg");

            if (selectedOps.Count != 1)
            {
                context.Console.Error.Write("Error: exactly one operation flag is required.\n");
                context.ExitCode = 1;
                return;
            }

            try
            {
                var connection = ResolveConnection(context, global, tcpOption, rtuOption, asciiOption);
                var logger = new VerbosityLogger(context.ParseResult.GetValueForOption(global.VerbosityLevel));
                logger.Log(1, $"Operation over {connection.Transport}");
                logger.Log(2, $"Endpoint: {connection.DisplayTarget}, unit={connection.SlaveId}, timeout={connection.Timeout}ms");
                logger.Log(3, $"Transport settings: baud={connection.SerialBaud}, dataBits={connection.SerialDataBits}, parity={connection.SerialParity}, stopBits={connection.SerialStopBits}");

                using var client = ModbusClientFactory.CreateClient(connection);

                var readCountDefault = context.ParseResult.GetValueForOption(countOption);
                var output = BuildOutputOptions(context, boolOption, bitsOption, hexOption, stringOption, stringLenOption, nullTermOption, encodingOption, bytesOption, quietOption, jsonOption, timestampOption, diffOption, onlyChangedOption, watchOption, monitorOption);

                if (context.ParseResult.GetValueForOption(readCoilOption) is { } readCoilSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readCoilSpec, readCountDefault);
                    await ExecuteReadLoop(async () => await client.ReadCoilsAsync(addr, cnt), values => RenderBoolValues(addr, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readDiscreteOption) is { } readDiscreteSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readDiscreteSpec, readCountDefault);
                    await ExecuteReadLoop(async () => await client.ReadDiscreteInputsAsync(addr, cnt), values => RenderBoolValues(addr, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readHoldingOption) is { } readHoldingSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readHoldingSpec, readCountDefault);
                    await ExecuteReadLoop(async () => await client.ReadHoldingRegistersAsync(addr, cnt), values => RenderRegisterValues(addr, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readInputRegOption) is { } readInputSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readInputSpec, readCountDefault);
                    await ExecuteReadLoop(async () => await client.ReadInputRegistersAsync(addr, cnt), values => RenderRegisterValues(addr, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readRefOption) is { } readRefSpec)
                {
                    var resolved = ParseReferenceSpec(readRefSpec, readCountDefault);
                    switch (resolved.ReferenceType)
                    {
                        case ReferenceType.Coil:
                            await ExecuteReadLoop(async () => await client.ReadCoilsAsync(resolved.Address, resolved.Count), values => RenderBoolValues(resolved.Address, values, output), output);
                            break;
                        case ReferenceType.Discrete:
                            await ExecuteReadLoop(async () => await client.ReadDiscreteInputsAsync(resolved.Address, resolved.Count), values => RenderBoolValues(resolved.Address, values, output), output);
                            break;
                        case ReferenceType.InputRegister:
                            await ExecuteReadLoop(async () => await client.ReadInputRegistersAsync(resolved.Address, resolved.Count), values => RenderRegisterValues(resolved.Address, values, output), output);
                            break;
                        case ReferenceType.HoldingRegister:
                            await ExecuteReadLoop(async () => await client.ReadHoldingRegistersAsync(resolved.Address, resolved.Count), values => RenderRegisterValues(resolved.Address, values, output), output);
                            break;
                    }

                    return;
                }

                var data = context.ParseResult.GetValueForOption(dataOption) ?? context.ParseResult.GetValueForOption(valueOption);

                if (context.ParseResult.GetValueForOption(writeCoilOption) is { } writeCoilAddr)
                {
                    var address = ParseWord(writeCoilAddr);
                    var value = ParseSingleCoilValue(data, context.ParseResult.GetValueForOption(onOption), context.ParseResult.GetValueForOption(offOption), context.ParseResult.GetValueForOption(raw16Option));
                    await client.WriteSingleCoilAsync(address, value);
                    Console.WriteLine($"Wrote coil {address}: {value}");
                    return;
                }

                if (context.ParseResult.GetValueForOption(writeRegOption) is { } writeRegAddr)
                {
                    var address = ParseWord(writeRegAddr);
                    var value = ParseSingleRegisterValue(data, context.ParseResult.GetValueForOption(raw16Option));
                    await client.WriteSingleRegisterAsync(address, value);
                    Console.WriteLine($"Wrote register {address}: {value}");
                    return;
                }

                if (context.ParseResult.GetValueForOption(writeMultiCoilOption) is { } writeMultiCoilAddr)
                {
                    var address = ParseWord(writeMultiCoilAddr);
                    var values = ParseMultiCoilValues(data, context.ParseResult.GetValueForOption(bitsOption), context.ParseResult.GetValueForOption(bytesOption));
                    await client.WriteMultipleCoilsAsync(address, values);
                    Console.WriteLine($"Wrote {values.Length} coils starting at {address}");
                    return;
                }

                if (context.ParseResult.GetValueForOption(writeMultiRegOption) is { } writeMultiRegAddr)
                {
                    var address = ParseWord(writeMultiRegAddr);
                    var values = ParseMultiRegisterValues(data);
                    await client.WriteMultipleRegistersAsync(address, values);
                    Console.WriteLine($"Wrote {values.Length} registers starting at {address}");
                }
            }
            catch (Exception ex)
            {
                context.Console.Error.Write($"Error: {ex.Message}\n");
                context.ExitCode = 1;
            }
        });
    }

    private static Option<double?> CreateOptionalIntervalOption(string alias, string description)
    {
        var option = new Option<double?>(alias, parseArgument: result =>
        {
            if (result.Tokens.Count == 0)
            {
                return 1.0;
            }

            return double.Parse(result.Tokens[0].Value, CultureInfo.InvariantCulture);
        })
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        option.Description = description;
        return option;
    }

    private static Option<string?> CreateOptionalStringOption(string alias, string description)
    {
        var option = new Option<string?>(alias, parseArgument: result =>
        {
            if (result.Tokens.Count == 0)
            {
                return string.Empty;
            }

            return result.Tokens[0].Value;
        })
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        option.Description = description;
        return option;
    }

    private static ResolvedConnection ResolveConnection(
        InvocationContext context,
        GlobalCliOptions global,
        Option<bool> tcpOption,
        Option<bool> rtuOption,
        Option<bool> asciiOption)
    {
        var explicitSerial = context.ParseResult.GetValueForOption(global.SerialPort);
        var target = context.ParseResult.GetValueForOption(global.Target);
        var protocol = context.ParseResult.GetValueForOption(global.Protocol);

        if (!string.IsNullOrWhiteSpace(explicitSerial))
        {
            target ??= explicitSerial;
            protocol ??= "rtu-serial";
        }

        if (context.ParseResult.GetValueForOption(tcpOption))
        {
            protocol = "tcp";
        }
        else if (context.ParseResult.GetValueForOption(rtuOption) && string.IsNullOrWhiteSpace(protocol))
        {
            protocol = IsSerialLike(target) ? "rtu-serial" : "rtu-tcp";
        }

        if (context.ParseResult.GetValueForOption(asciiOption))
        {
            throw new NotSupportedException("--ascii mode is a milestone flag and is not implemented yet.");
        }

        if (string.IsNullOrWhiteSpace(target) && string.IsNullOrWhiteSpace(explicitSerial))
        {
            throw new ArgumentException("TARGET is required unless --serial is provided.");
        }

        var timeoutMilliseconds = (int)Math.Round(context.ParseResult.GetValueForOption(global.TimeoutSeconds) * 1000.0);

        var options = new ConnectionOptions(
            Target: target,
            Host: context.ParseResult.GetValueForOption(global.Host),
            Port: context.ParseResult.GetValueForOption(global.Port),
            SlaveId: context.ParseResult.GetValueForOption(global.SlaveId),
            Timeout: timeoutMilliseconds,
            Retries: context.ParseResult.GetValueForOption(global.Retries),
            Protocol: protocol,
            SerialPort: explicitSerial,
            SerialBaud: context.ParseResult.GetValueForOption(global.SerialBaud),
            SerialDataBits: context.ParseResult.GetValueForOption(global.SerialDataBits),
            SerialParity: context.ParseResult.GetValueForOption(global.SerialParity)!,
            SerialStopBits: context.ParseResult.GetValueForOption(global.SerialStopBits)!);

        return EndpointResolver.Resolve(options);
    }

    private static (ushort Address, ushort Count) ParseReadSpec(string spec, ushort fallbackCount)
    {
        var parts = spec.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length == 1)
        {
            return (ParseWord(parts[0]), fallbackCount);
        }

        if (parts.Length == 2)
        {
            return (ParseWord(parts[0]), ParseWord(parts[1]));
        }

        throw new ArgumentException($"Invalid SPEC format '{spec}'. Use ADDR or ADDR:COUNT.");
    }

    private static ReferenceReadRequest ParseReferenceSpec(string spec, ushort fallbackCount)
    {
        var (reference, count) = ParseReadSpec(spec, fallbackCount);

        if (reference >= 1 && reference <= 9999)
        {
            return new ReferenceReadRequest(ReferenceType.Coil, (ushort)(reference - 1), count);
        }

        if (reference >= 10001 && reference <= 19999)
        {
            return new ReferenceReadRequest(ReferenceType.Discrete, (ushort)(reference - 10001), count);
        }

        if (reference >= 30001 && reference <= 39999)
        {
            return new ReferenceReadRequest(ReferenceType.InputRegister, (ushort)(reference - 30001), count);
        }

        if (reference >= 40001 && reference <= 49999)
        {
            return new ReferenceReadRequest(ReferenceType.HoldingRegister, (ushort)(reference - 40001), count);
        }

        throw new ArgumentException($"Unsupported reference range: {reference}");
    }

    private static ushort ParseWord(string value)
    {
        var input = value.Trim();

        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return Convert.ToUInt16(input[2..], 16);
        }

        if (input.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
        {
            return Convert.ToUInt16(input[2..], 2);
        }

        return Convert.ToUInt16(input, 10);
    }

    private static bool ParseSingleCoilValue(string? data, bool on, bool off, string? raw16)
    {
        if (on && off)
        {
            throw new ArgumentException("--on and --off cannot be used together.");
        }

        if (on)
        {
            return true;
        }

        if (off)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(raw16))
        {
            var payload = ParseWord(raw16);
            return payload switch
            {
                0xFF00 => true,
                0x0000 => false,
                _ => throw new ArgumentException("--raw16 for coil must be 0xFF00 or 0x0000")
            };
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data/--value (or --on/--off) is required for --write-coil");
        }

        if (bool.TryParse(data, out var boolValue))
        {
            return boolValue;
        }

        return ParseWord(data) != 0;
    }

    private static ushort ParseSingleRegisterValue(string? data, string? raw16)
    {
        if (!string.IsNullOrWhiteSpace(raw16))
        {
            return ParseWord(raw16);
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data/--value is required for --write-reg");
        }

        return ParseWord(data);
    }

    private static bool[] ParseMultiCoilValues(string? data, string? bitsPayload, string? bytesPayload)
    {
        if (!string.IsNullOrWhiteSpace(bitsPayload))
        {
            if (bitsPayload.Contains(',', StringComparison.Ordinal))
            {
                return bitsPayload
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(v => ParseWord(v) != 0)
                    .ToArray();
            }

            return bitsPayload.Trim().Select(ch => ch != '0').ToArray();
        }

        if (!string.IsNullOrWhiteSpace(bytesPayload))
        {
            var bytes = ParseByteList(bytesPayload);
            var values = new List<bool>();
            foreach (var b in bytes)
            {
                for (var bit = 0; bit < 8; bit++)
                {
                    values.Add(((b >> bit) & 0x1) == 1);
                }
            }

            return values.ToArray();
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data/--bits/--bytes is required for --write-multi-coil");
        }

        if (data.Contains(',', StringComparison.Ordinal))
        {
            return data
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(v => ParseWord(v) != 0)
                .ToArray();
        }

        if (data.All(char.IsDigit))
        {
            return data.Select(ch => ch != '0').ToArray();
        }

        throw new ArgumentException("Invalid --data for --write-multi-coil");
    }

    private static ushort[] ParseMultiRegisterValues(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("--data/--value is required for --write-multi-reg");
        }

        return data
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParseWord)
            .ToArray();
    }

    private static byte[] ParseByteList(string input)
    {
        return input
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(token => (byte)ParseWord(token))
            .ToArray();
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

    private static OutputOptions BuildOutputOptions(
        InvocationContext context,
        Option<bool> boolOption,
        Option<string?> bitsOption,
        Option<bool> hexOption,
        Option<bool> stringOption,
        Option<int?> stringLenOption,
        Option<bool> nullTermOption,
        Option<string> encodingOption,
        Option<string?> bytesOption,
        Option<bool> quietOption,
        Option<bool> jsonOption,
        Option<bool> timestampOption,
        Option<bool> diffOption,
        Option<bool> onlyChangedOption,
        Option<double?> watchOption,
        Option<double?> monitorOption)
    {
        var watchSeconds = context.ParseResult.GetValueForOption(watchOption);
        var monitorSeconds = context.ParseResult.GetValueForOption(monitorOption);

        var bitsSpecified = context.ParseResult.FindResultFor(bitsOption) is not null;
        var bitsHasValue = !string.IsNullOrWhiteSpace(context.ParseResult.GetValueForOption(bitsOption));
        var bytesSpecified = context.ParseResult.FindResultFor(bytesOption) is not null;
        var bytesHasValue = !string.IsNullOrWhiteSpace(context.ParseResult.GetValueForOption(bytesOption));

        return new OutputOptions(
            ShowBool: context.ParseResult.GetValueForOption(boolOption) || !bitsSpecified,
            ShowBits: bitsSpecified && !bitsHasValue,
            ShowHex: context.ParseResult.GetValueForOption(hexOption),
            DecodeString: context.ParseResult.GetValueForOption(stringOption),
            StringLength: context.ParseResult.GetValueForOption(stringLenOption),
            NullTerminate: context.ParseResult.GetValueForOption(nullTermOption),
            EncodingName: context.ParseResult.GetValueForOption(encodingOption)!,
            ShowBytes: bytesSpecified && !bytesHasValue,
            Quiet: context.ParseResult.GetValueForOption(quietOption),
            Json: context.ParseResult.GetValueForOption(jsonOption),
            Timestamp: context.ParseResult.GetValueForOption(timestampOption),
            Diff: context.ParseResult.GetValueForOption(diffOption),
            OnlyChanged: context.ParseResult.GetValueForOption(onlyChangedOption),
            WatchIntervalSeconds: watchSeconds,
            MonitorIntervalSeconds: monitorSeconds);
    }

    private static async Task ExecuteReadLoop<T>(
        Func<Task<T>> readFunc,
        Action<T> render,
        OutputOptions output)
    {
        var interval = output.MonitorIntervalSeconds ?? output.WatchIntervalSeconds;
        if (interval is null)
        {
            var result = await readFunc();
            render(result);
            return;
        }

        var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        string[] previousLines = Array.Empty<string>();

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var result = await readFunc();

            if (output.MonitorIntervalSeconds is not null)
            {
                Console.Clear();
            }

            var originalOut = Console.Out;
            using var buffer = new StringWriter();
            Console.SetOut(buffer);
            render(result);
            Console.SetOut(originalOut);

            var lines = buffer.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.TrimEnd('\r')).ToArray();
            var projected = ApplyDiffFiltering(lines, previousLines, output);
            foreach (var line in projected)
            {
                Console.WriteLine(output.Timestamp ? $"{DateTimeOffset.Now:O} {line}" : line);
            }

            previousLines = lines;

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(interval.Value), cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private static IEnumerable<string> ApplyDiffFiltering(string[] current, string[] previous, OutputOptions output)
    {
        if (!output.Diff && !output.OnlyChanged)
        {
            return current;
        }

        var changed = current
            .Select((line, i) => new { line, i })
            .Where(x => x.i >= previous.Length || !string.Equals(x.line, previous[x.i], StringComparison.Ordinal))
            .ToArray();

        if (output.OnlyChanged)
        {
            return changed.Select(x => x.line);
        }

        var changedIndexes = changed.Select(x => x.i).ToHashSet();
        return current.Select((line, i) => changedIndexes.Contains(i) ? $"* {line}" : line);
    }

    private static void WriteValueLines(IReadOnlyList<string> lines, bool multiColumnForWatch)
    {
        if (lines.Count == 0)
        {
            return;
        }

        if (!multiColumnForWatch)
        {
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            return;
        }

        var availableHeight = GetConsoleHeight();
        if (lines.Count <= availableHeight)
        {
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            return;
        }

        var columns = (lines.Count + availableHeight - 1) / availableHeight;
        var widths = new int[columns];
        for (var col = 0; col < columns; col++)
        {
            var columnStart = col * availableHeight;
            var columnEnd = Math.Min(columnStart + availableHeight, lines.Count);
            for (var index = columnStart; index < columnEnd; index++)
            {
                widths[col] = Math.Max(widths[col], lines[index].Length);
            }
        }

        const int ColumnPadding = 2;
        for (var row = 0; row < availableHeight; row++)
        {
            var rowBuilder = new StringBuilder();
            var printedAny = false;
            for (var col = 0; col < columns; col++)
            {
                var index = col * availableHeight + row;
                if (index >= lines.Count)
                {
                    continue;
                }

                var entry = lines[index];
                var padding = col == columns - 1 ? 0 : ColumnPadding;
                rowBuilder.Append(col == columns - 1 ? entry : entry.PadRight(widths[col] + padding));
                printedAny = true;
            }

            if (printedAny)
            {
                Console.WriteLine(rowBuilder.ToString());
            }
        }
    }

    private static int GetConsoleHeight()
    {
        try
        {
            var height = Math.Max(Console.WindowHeight - 1, 1);
            return height > 0 ? height : 1;
        }
        catch (IOException)
        {
            return 20;
        }
        catch (PlatformNotSupportedException)
        {
            return 20;
        }
    }

    private static void RenderBoolValues(ushort startAddress, bool[] values, OutputOptions output)
    {
        if (output.Json)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { startAddress, values }));
            return;
        }

        if (output.ShowBits)
        {
            Console.WriteLine(string.Concat(values.Select(v => v ? '1' : '0')));
            return;
        }

        if (output.ShowHex)
        {
            Console.WriteLine(BitConverter.ToString(PackBits(values)).Replace("-", string.Empty, StringComparison.Ordinal));
            return;
        }

        var lines = new List<string>(values.Length);
        for (var i = 0; i < values.Length; i++)
        {
            var valueText = output.ShowBool ? values[i].ToString() : (values[i] ? "1" : "0");
            lines.Add(output.Quiet ? valueText : $"{startAddress + i}: {valueText}");
        }

        WriteValueLines(lines, output.WatchIntervalSeconds is not null && output.MonitorIntervalSeconds is null);
    }

    private static void RenderRegisterValues(ushort startAddress, ushort[] values, OutputOptions output)
    {
        if (output.Json)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { startAddress, values }));
            return;
        }

        if (output.ShowBytes || output.DecodeString)
        {
            var bytes = ToBytes(values);
            if (output.ShowBytes)
            {
                Console.WriteLine(BitConverter.ToString(bytes).Replace("-", string.Empty, StringComparison.Ordinal));
            }

            if (output.DecodeString)
            {
                var length = output.StringLength ?? bytes.Length;
                length = Math.Min(length, bytes.Length);

                if (output.NullTerminate)
                {
                    var nullIndex = Array.IndexOf(bytes, (byte)0, 0, length);
                    if (nullIndex >= 0)
                    {
                        length = nullIndex;
                    }
                }

                var encoding = Encoding.GetEncoding(output.EncodingName);
                Console.WriteLine(encoding.GetString(bytes, 0, length));
            }

            return;
        }

        var lines = new List<string>(values.Length);
        for (var i = 0; i < values.Length; i++)
        {
            var entry = output.Quiet ? values[i].ToString() : $"{startAddress + i}: {values[i]}";
            lines.Add(entry);
        }

        WriteValueLines(lines, output.WatchIntervalSeconds is not null && output.MonitorIntervalSeconds is null);
    }

    private static byte[] ToBytes(ushort[] values)
    {
        var bytes = new byte[values.Length * 2];
        for (var i = 0; i < values.Length; i++)
        {
            bytes[i * 2] = (byte)(values[i] >> 8);
            bytes[i * 2 + 1] = (byte)(values[i] & 0xFF);
        }

        return bytes;
    }

    private static byte[] PackBits(bool[] values)
    {
        var byteCount = (values.Length + 7) / 8;
        var bytes = new byte[byteCount];

        for (var i = 0; i < values.Length; i++)
        {
            if (values[i])
            {
                bytes[i / 8] |= (byte)(1 << (i % 8));
            }
        }

        return bytes;
    }

    private enum ReferenceType
    {
        Coil,
        Discrete,
        InputRegister,
        HoldingRegister
    }

    private sealed record ReferenceReadRequest(ReferenceType ReferenceType, ushort Address, ushort Count);

    private sealed record OutputOptions(
        bool ShowBool,
        bool ShowBits,
        bool ShowHex,
        bool DecodeString,
        int? StringLength,
        bool NullTerminate,
        string EncodingName,
        bool ShowBytes,
        bool Quiet,
        bool Json,
        bool Timestamp,
        bool Diff,
        bool OnlyChanged,
        double? WatchIntervalSeconds,
        double? MonitorIntervalSeconds);
}
