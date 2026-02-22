using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using ModbusDirekt.Exceptions;
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
        var analyzeOption = new Option<bool>("--analyze", "Probe FC01-04 address spaces and report overlap/mirroring");
        var diffOption = new Option<bool>("--diff", "Highlight changed values");
        var onlyChangedOption = new Option<bool>("--only-changed", "Only show changed values");
        var groupOption = new Option<int?>("--group", "Group output values");
        var scanOption = CreateOptionalIntervalOption("--scan", "Live multi-FC scan dashboard (optional interval seconds)");

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
        rootCommand.AddOption(analyzeOption);
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
            if (context.ParseResult.GetValueForOption(analyzeOption)) selectedOps.Add("analyze");
            if (context.ParseResult.GetValueForOption(scanOption) is not null) selectedOps.Add("scan");

            if (selectedOps.Count != 1)
            {
                context.Console.Error.Write("Error: exactly one operation flag is required.\n");
                context.ExitCode = 1;
                return;
            }

            ResolvedConnection? resolvedConnection = null;
            try
            {
                resolvedConnection = ResolveConnection(context, global, tcpOption, rtuOption, asciiOption);
                var logger = new VerbosityLogger(context.ParseResult.GetValueForOption(global.VerbosityLevel));
                logger.Log(1, $"Operation over {resolvedConnection.Transport}");
                logger.Log(2, $"Endpoint: {resolvedConnection.DisplayTarget}, unit={resolvedConnection.SlaveId}, timeout={resolvedConnection.Timeout}ms");
                logger.Log(3, $"Transport settings: baud={resolvedConnection.SerialBaud}, dataBits={resolvedConnection.SerialDataBits}, parity={resolvedConnection.SerialParity}, stopBits={resolvedConnection.SerialStopBits}");

                using var client = ModbusClientFactory.CreateClient(resolvedConnection);

                if (context.ParseResult.GetValueForOption(analyzeOption))
                {
                    await ExecuteAnalyzeMode(client, resolvedConnection.Transport);
                    return;
                }

                if (context.ParseResult.GetValueForOption(scanOption) is { } scanIntervalSeconds)
                {
                    await ExecuteScanMode(client, resolvedConnection.Transport, scanIntervalSeconds);
                    return;
                }

                var readCountDefault = context.ParseResult.GetValueForOption(countOption);
                var output = BuildOutputOptions(context, boolOption, bitsOption, hexOption, stringOption, stringLenOption, nullTermOption, encodingOption, bytesOption, quietOption, jsonOption, timestampOption, diffOption, onlyChangedOption, watchOption, monitorOption);

                if (context.ParseResult.GetValueForOption(readCoilOption) is { } readCoilSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readCoilSpec, readCountDefault);
                    ValidateReadRange(addr, cnt, ProtocolLimits.MaxBitsPerRead, "FC01/read-coil");
                    await ExecuteReadLoop(async () => await client.ReadCoilsAsync(addr, cnt), values => RenderBoolValues(addr, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readDiscreteOption) is { } readDiscreteSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readDiscreteSpec, readCountDefault);
                    ValidateReadRange(addr, cnt, ProtocolLimits.MaxBitsPerRead, "FC02/read-discrete");
                    await ExecuteReadLoop(async () => await client.ReadDiscreteInputsAsync(addr, cnt), values => RenderBoolValues(addr, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readHoldingOption) is { } readHoldingSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readHoldingSpec, readCountDefault);
                    ValidateReadRange(addr, cnt, ProtocolLimits.MaxRegistersPerRead, "FC03/read-holding");
                    await ExecuteReadLoop(async () => await client.ReadHoldingRegistersAsync(addr, cnt), values => RenderRegisterValues(addr, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readInputRegOption) is { } readInputSpec)
                {
                    var (addr, cnt) = ParseReadSpec(readInputSpec, readCountDefault);
                    ValidateReadRange(addr, cnt, ProtocolLimits.MaxRegistersPerRead, "FC04/read-inputreg");
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
                var category = ClassifyErrorCategory(ex, resolvedConnection?.Transport);
                context.Console.Error.Write($"Error ({DescribeErrorCategory(category)}): {FormatErrorMessage(ex)}\n");
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
            var address = (ushort)(reference - 1);
            ValidateReadRange(address, count, ProtocolLimits.MaxBitsPerRead, "FC01/ref");
            return new ReferenceReadRequest(ReferenceType.Coil, address, count);
        }

        if (reference >= 10001 && reference <= 19999)
        {
            var address = (ushort)(reference - 10001);
            ValidateReadRange(address, count, ProtocolLimits.MaxBitsPerRead, "FC02/ref");
            return new ReferenceReadRequest(ReferenceType.Discrete, address, count);
        }

        if (reference >= 30001 && reference <= 39999)
        {
            var address = (ushort)(reference - 30001);
            ValidateReadRange(address, count, ProtocolLimits.MaxRegistersPerRead, "FC04/ref");
            return new ReferenceReadRequest(ReferenceType.InputRegister, address, count);
        }

        if (reference >= 40001 && reference <= 49999)
        {
            var address = (ushort)(reference - 40001);
            ValidateReadRange(address, count, ProtocolLimits.MaxRegistersPerRead, "FC03/ref");
            return new ReferenceReadRequest(ReferenceType.HoldingRegister, address, count);
        }

        throw new ArgumentException($"Unsupported reference range: {reference}");
    }

    private static void ValidateReadRange(ushort address, ushort count, int protocolMaxCount, string operationName)
    {
        if (count == 0)
        {
            throw new ArgumentException($"Invalid count 0 for {operationName}. Count must be at least 1.");
        }

        if (count > protocolMaxCount)
        {
            throw new ArgumentException($"Invalid count {count} for {operationName}. Protocol maximum is {protocolMaxCount}.");
        }

        var upperAddress = address + count - 1;
        if (upperAddress > ProtocolLimits.MaxAddress)
        {
            throw new ArgumentException($"Invalid range for {operationName}: start={address}, count={count}, upper={upperAddress}. Address limit is {ProtocolLimits.MaxAddress}.");
        }
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

    private static async Task ExecuteAnalyzeMode(IModbusClient client, TransportKind transportKind)
    {
        Console.WriteLine("Analyze mode: probing FC01-FC04 address spaces...");
        var discoveredSpaces = await DiscoverAllAddressSpacesAsync(client, transportKind);

        Console.WriteLine();
        Console.WriteLine("Address-space summary:");
        foreach (var space in discoveredSpaces)
        {
            if (space.MaxAddress >= 0)
            {
                Console.WriteLine($"  {space.Spec.CodeLabel}: 0..{space.MaxAddress} ({space.MaxAddress + 1} cells)");
            }
            else
            {
                Console.WriteLine($"  {space.Spec.CodeLabel}: no readable cells ({space.BoundaryReason ?? "no successful read"})");
            }
        }

        var captures = new List<FunctionCodeCapture>();
        foreach (var space in discoveredSpaces.Where(s => s.MaxAddress >= 0))
        {
            var firstSnapshot = await ReadAddressSpaceSnapshotAsync(client, space.Spec, space.MaxAddress, 1, showProgress: true);
            var secondSnapshot = await ReadAddressSpaceSnapshotAsync(client, space.Spec, space.MaxAddress, 2, showProgress: true);
            var volatileMask = BuildVolatileMask(firstSnapshot, secondSnapshot);
            captures.Add(new FunctionCodeCapture(space, secondSnapshot, volatileMask));
        }

        Console.WriteLine();
        Console.WriteLine("Volatility summary:");
        foreach (var capture in captures)
        {
            var volatileCount = capture.VolatileMask.Count(changed => changed);
            Console.WriteLine($"  {capture.Space.Spec.CodeLabel}: volatile {volatileCount}/{capture.Values.Length}");
        }

        Console.WriteLine();
        Console.WriteLine("Mirror/overlap cross-check:");
        for (var i = 0; i < captures.Count; i++)
        {
            for (var j = i + 1; j < captures.Count; j++)
            {
                PrintAddressSpaceComparison(captures[i], captures[j]);
            }
        }
    }

    private static async Task ExecuteScanMode(IModbusClient client, TransportKind transportKind, double intervalSeconds)
    {
        if (intervalSeconds <= 0)
        {
            throw new ArgumentException("--scan interval must be greater than 0.");
        }

        Console.WriteLine("Scan mode: probing FC01-FC04 address spaces...");
        var discoveredSpaces = await DiscoverAllAddressSpacesAsync(client, transportKind);

        var states = new Dictionary<string, ScanState>(StringComparer.Ordinal);
        foreach (var space in discoveredSpaces.Where(s => s.MaxAddress >= 0))
        {
            var initial = await ReadAddressSpaceSnapshotAsync(client, space.Spec, space.MaxAddress, 0, showProgress: true);
            states[space.Spec.CodeLabel] = new ScanState(space, initial);
        }

        if (states.Count == 0)
        {
            Console.WriteLine("No readable function-code tables were discovered.");
            return;
        }

        Console.WriteLine("Starting live scan dashboard (Ctrl+C to stop)...");
        var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        var cycle = 0;
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            cycle++;
            var changedNowByCode = new Dictionary<string, HashSet<int>>(StringComparer.Ordinal);

            foreach (var spec in FunctionCodeSpecs)
            {
                if (!states.TryGetValue(spec.CodeLabel, out var state))
                {
                    continue;
                }

                var current = await ReadAddressSpaceSnapshotAsync(client, spec, state.Space.MaxAddress, cycle, showProgress: false);
                var changedNow = new HashSet<int>();
                for (var address = 0; address < current.Length; address++)
                {
                    if (!string.Equals(current[address], state.PreviousValues[address], StringComparison.Ordinal))
                    {
                        changedNow.Add(address);
                        state.ChangedEver.Add(address);
                    }
                }

                changedNowByCode[spec.CodeLabel] = changedNow;
                state.PreviousValues = current;
            }

            RenderScanDashboard(states, changedNowByCode, cycle);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private static async Task<IReadOnlyList<FunctionCodeSpace>> DiscoverAllAddressSpacesAsync(IModbusClient client, TransportKind transportKind)
    {
        var spaces = new List<FunctionCodeSpace>(FunctionCodeSpecs.Length);
        foreach (var spec in FunctionCodeSpecs)
        {
            spaces.Add(await DiscoverAddressSpaceAsync(client, spec, transportKind));
        }

        return spaces;
    }

    private static async Task<FunctionCodeSpace> DiscoverAddressSpaceAsync(
        IModbusClient client,
        FunctionCodeSpec spec,
        TransportKind transportKind)
    {
        var probeWindow = Math.Min(ProtocolLimits.InitialProbeWindow, spec.MaxReadCount);
        var stepSize = Math.Max(1, probeWindow / 2);
        var probeUpperAddress = Math.Min(ProtocolLimits.MaxAddress, probeWindow - 1);

        var maxSuccessfulAddress = -1;
        var firstFailedAddress = ProtocolLimits.MaxAddress + 1;
        string? boundaryReason = null;

        while (probeUpperAddress <= ProtocolLimits.MaxAddress)
        {
            var probe = await TryProbeReadUpToAsync(client, spec, probeUpperAddress, probeWindow, transportKind);
            WriteProbeProgress(spec, "step", probeUpperAddress, probe, probeWindow);
            if (probe.Success)
            {
                maxSuccessfulAddress = probeUpperAddress;
                if (probeUpperAddress == ProtocolLimits.MaxAddress)
                {
                    break;
                }

                var nextProbe = (long)probeUpperAddress + stepSize;
                probeUpperAddress = (int)Math.Min(ProtocolLimits.MaxAddress, nextProbe);
                continue;
            }

            firstFailedAddress = probeUpperAddress;
            boundaryReason = probe.FailureReason;
            break;
        }

        if (maxSuccessfulAddress == ProtocolLimits.MaxAddress)
        {
            Console.WriteLine($"[{spec.CodeLabel}] discovered: 0..{ProtocolLimits.MaxAddress}");
            return new FunctionCodeSpace(spec, ProtocolLimits.MaxAddress, null);
        }

        if (firstFailedAddress == ProtocolLimits.MaxAddress + 1)
        {
            firstFailedAddress = ProtocolLimits.MaxAddress;
        }

        var low = maxSuccessfulAddress + 1;
        var high = Math.Max(low - 1, firstFailedAddress - 1);
        while (low <= high)
        {
            var mid = low + ((high - low) / 2);
            var probe = await TryProbeReadUpToAsync(client, spec, mid, probeWindow, transportKind);
            WriteProbeProgress(spec, "bisect", mid, probe, probeWindow);
            if (probe.Success)
            {
                maxSuccessfulAddress = mid;
                low = mid + 1;
                continue;
            }

            boundaryReason = probe.FailureReason;
            high = mid - 1;
        }

        if (maxSuccessfulAddress >= 0)
        {
            Console.WriteLine($"[{spec.CodeLabel}] discovered: 0..{maxSuccessfulAddress}");
        }
        else
        {
            Console.WriteLine($"[{spec.CodeLabel}] discovered: no readable addresses");
        }

        return new FunctionCodeSpace(spec, maxSuccessfulAddress, boundaryReason);
    }

    private static async Task<ProbeReadResult> TryProbeReadUpToAsync(
        IModbusClient client,
        FunctionCodeSpec spec,
        int upperAddress,
        int probeWindow,
        TransportKind transportKind)
    {
        var startAddress = Math.Max(0, upperAddress - probeWindow + 1);
        var count = upperAddress - startAddress + 1;
        ValidateReadRange((ushort)startAddress, (ushort)count, spec.MaxReadCount, spec.CodeLabel);

        ProbeFailure? lastFailure = null;
        for (var attempt = 1; attempt <= ProbeMaxAttempts; attempt++)
        {
            try
            {
                await spec.ProbeReadAsync(client, (ushort)startAddress, (ushort)count);
                return new ProbeReadResult(startAddress, count, Success: true, FailureReason: null);
            }
            catch (Exception ex)
            {
                var category = ClassifyErrorCategory(ex, transportKind);
                if (IsFatalProbeError(category))
                {
                    throw new InvalidOperationException(
                        $"Probe for {spec.CodeLabel} at start={startAddress}, count={count} failed due to {DescribeErrorCategory(category)}.",
                        ex);
                }

                lastFailure = new ProbeFailure(category, FormatErrorMessage(ex));
            }
        }

        if (lastFailure is null)
        {
            throw new InvalidOperationException("Probe failed without a captured error.");
        }

        var reason = $"{DescribeErrorCategory(lastFailure.Category)}: {lastFailure.Message}";
        return new ProbeReadResult(startAddress, count, Success: false, FailureReason: reason);
    }

    private static async Task<string[]> ReadAddressSpaceSnapshotAsync(
        IModbusClient client,
        FunctionCodeSpec spec,
        int maxAddress,
        int pass,
        bool showProgress)
    {
        if (maxAddress < 0)
        {
            return Array.Empty<string>();
        }

        var values = new string[maxAddress + 1];
        var total = maxAddress + 1;
        for (var startAddress = 0; startAddress <= maxAddress; startAddress += spec.MaxReadCount)
        {
            var count = Math.Min(spec.MaxReadCount, (maxAddress - startAddress) + 1);
            var chunk = await spec.ReadValuesAsync(client, (ushort)startAddress, (ushort)count);
            if (chunk.Length < count)
            {
                throw new InvalidOperationException($"{spec.CodeLabel} snapshot read returned {chunk.Length} values, expected {count}.");
            }

            Array.Copy(chunk, 0, values, startAddress, count);

            if (showProgress)
            {
                var completed = Math.Min(total, startAddress + count);
                Console.WriteLine($"[{spec.CodeLabel}] pass {pass}: {completed}/{total}");
            }
        }

        return values;
    }

    private static bool[] BuildVolatileMask(string[] firstSnapshot, string[] secondSnapshot)
    {
        var length = Math.Min(firstSnapshot.Length, secondSnapshot.Length);
        var volatileMask = new bool[length];
        for (var i = 0; i < length; i++)
        {
            volatileMask[i] = !string.Equals(firstSnapshot[i], secondSnapshot[i], StringComparison.Ordinal);
        }

        return volatileMask;
    }

    private static void PrintAddressSpaceComparison(FunctionCodeCapture left, FunctionCodeCapture right)
    {
        var overlapMaxAddress = Math.Min(left.Space.MaxAddress, right.Space.MaxAddress);
        if (overlapMaxAddress < 0)
        {
            Console.WriteLine($"  {left.Space.Spec.CodeLabel} vs {right.Space.Spec.CodeLabel}: no address overlap");
            return;
        }

        var comparableCount = 0;
        var equalCount = 0;
        for (var address = 0; address <= overlapMaxAddress; address++)
        {
            if (address >= left.VolatileMask.Length || address >= right.VolatileMask.Length)
            {
                break;
            }

            if (left.VolatileMask[address] || right.VolatileMask[address])
            {
                continue;
            }

            comparableCount++;
            if (string.Equals(left.Values[address], right.Values[address], StringComparison.Ordinal))
            {
                equalCount++;
            }
        }

        if (comparableCount == 0)
        {
            Console.WriteLine($"  {left.Space.Spec.CodeLabel} vs {right.Space.Spec.CodeLabel}: overlap 0..{overlapMaxAddress}, but no stable cells to compare");
            return;
        }

        var equalRatio = (double)equalCount / comparableCount;
        var verdict = equalRatio switch
        {
            >= 0.95 => "likely mirrored",
            >= 0.60 => "partial overlap",
            _ => "no strong mirror"
        };

        Console.WriteLine($"  {left.Space.Spec.CodeLabel} vs {right.Space.Spec.CodeLabel}: overlap 0..{overlapMaxAddress}, stable={comparableCount}, equal={equalCount} ({equalRatio:P1}) => {verdict}");
    }

    private static void RenderScanDashboard(
        IReadOnlyDictionary<string, ScanState> states,
        IReadOnlyDictionary<string, HashSet<int>> changedNowByCode,
        int cycle)
    {
        Console.Clear();
        Console.WriteLine($"Scan dashboard | {DateTimeOffset.Now:O} | cycle {cycle}");

        var columnHeaders = new List<string>(FunctionCodeSpecs.Length);
        foreach (var spec in FunctionCodeSpecs)
        {
            if (states.TryGetValue(spec.CodeLabel, out var state))
            {
                columnHeaders.Add($"{spec.CodeLabel} 0..{state.Space.MaxAddress} tracked={state.ChangedEver.Count}");
            }
            else
            {
                columnHeaders.Add($"{spec.CodeLabel} unsupported");
            }
        }

        const int columnWidth = 32;
        Console.WriteLine(string.Join(" | ", columnHeaders.Select(h => PadCell(h, columnWidth))));

        var rowsByColumn = new List<string>[FunctionCodeSpecs.Length];
        for (var i = 0; i < FunctionCodeSpecs.Length; i++)
        {
            var spec = FunctionCodeSpecs[i];
            if (!states.TryGetValue(spec.CodeLabel, out var state))
            {
                rowsByColumn[i] = new List<string> { "(n/a)" };
                continue;
            }

            if (state.ChangedEver.Count == 0)
            {
                rowsByColumn[i] = new List<string> { "(no changes yet)" };
                continue;
            }

            changedNowByCode.TryGetValue(spec.CodeLabel, out var changedNow);
            var rows = state.ChangedEver
                .OrderBy(address => address)
                .Select(address =>
                {
                    var marker = changedNow is not null && changedNow.Contains(address) ? "*" : " ";
                    return $"{marker}{address}:{state.PreviousValues[address]}";
                })
                .ToList();

            rowsByColumn[i] = rows;
        }

        var maxRows = rowsByColumn.Max(rows => rows.Count);
        for (var rowIndex = 0; rowIndex < maxRows; rowIndex++)
        {
            var row = new string[FunctionCodeSpecs.Length];
            for (var columnIndex = 0; columnIndex < FunctionCodeSpecs.Length; columnIndex++)
            {
                var columnRows = rowsByColumn[columnIndex];
                var text = rowIndex < columnRows.Count ? columnRows[rowIndex] : string.Empty;
                row[columnIndex] = PadCell(text, columnWidth);
            }

            Console.WriteLine(string.Join(" | ", row));
        }
    }

    private static void WriteProbeProgress(FunctionCodeSpec spec, string phase, int upperAddress, ProbeReadResult probe, int window)
    {
        var status = probe.Success ? "ok" : $"fail ({probe.FailureReason})";
        Console.WriteLine($"[{spec.CodeLabel}] {phase} upper={upperAddress} start={probe.StartAddress} count={probe.Count} {spec.CountUnitLabel} window={window}: {status}");
    }

    private static string PadCell(string value, int width)
    {
        if (value.Length <= width)
        {
            return value.PadRight(width);
        }

        if (width <= 3)
        {
            return value[..width];
        }

        return $"{value[..(width - 3)]}...";
    }

    private static CliErrorCategory ClassifyErrorCategory(Exception ex, TransportKind? transportKind)
    {
        var root = GetRootCause(ex);
        if (root is ModbusException modbusException)
        {
            return modbusException.ExceptionCode is 10 or 11
                ? CliErrorCategory.Gateway
                : CliErrorCategory.ModbusDevice;
        }

        if (IsFrameLevelError(root))
        {
            return CliErrorCategory.ProtocolFrame;
        }

        if (root is TimeoutException or IOException or SocketException)
        {
            return transportKind switch
            {
                TransportKind.RtuSerial => CliErrorCategory.SerialTransport,
                TransportKind.RtuTcp => CliErrorCategory.RtuTcpTransport,
                TransportKind.Tcp => CliErrorCategory.ModbusTcpTransport,
                _ => CliErrorCategory.Transport
            };
        }

        if (root is ArgumentException or FormatException or OverflowException)
        {
            return CliErrorCategory.Parameter;
        }

        return CliErrorCategory.Runtime;
    }

    private static bool IsFrameLevelError(Exception ex)
    {
        var message = ex.Message.Trim();
        return message.Contains("invalid crc", StringComparison.OrdinalIgnoreCase)
            || message.Contains("invalid frame", StringComparison.OrdinalIgnoreCase)
            || message.Contains("invalid response", StringComparison.OrdinalIgnoreCase)
            || message.Contains("incomplete response", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsFatalProbeError(CliErrorCategory category)
    {
        return category is CliErrorCategory.Parameter or CliErrorCategory.Runtime;
    }

    private static string DescribeErrorCategory(CliErrorCategory category)
    {
        return category switch
        {
            CliErrorCategory.Parameter => "Parameter",
            CliErrorCategory.SerialTransport => "SerialTransport",
            CliErrorCategory.ModbusTcpTransport => "ModbusTcpTransport",
            CliErrorCategory.RtuTcpTransport => "RtuTcpTransport",
            CliErrorCategory.Transport => "Transport",
            CliErrorCategory.Gateway => "Gateway",
            CliErrorCategory.ModbusDevice => "ModbusDevice",
            CliErrorCategory.ProtocolFrame => "ProtocolFrame",
            _ => "Runtime"
        };
    }

    private static string FormatErrorMessage(Exception ex)
    {
        var root = GetRootCause(ex);
        if (root is ModbusException modbusException)
        {
            var codeName = DescribeModbusExceptionCode(modbusException.ExceptionCode);
            return $"{root.Message} (exception-code {modbusException.ExceptionCode}: {codeName})";
        }

        return root.Message;
    }

    private static Exception GetRootCause(Exception ex)
    {
        var current = ex;
        while (current.InnerException is not null)
        {
            current = current.InnerException;
        }

        return current;
    }

    private static string DescribeModbusExceptionCode(int code)
    {
        return code switch
        {
            1 => "ILLEGAL_FUNCTION",
            2 => "ILLEGAL_DATA_ADDRESS",
            3 => "ILLEGAL_DATA_VALUE",
            4 => "SERVER_DEVICE_FAILURE",
            5 => "ACKNOWLEDGE",
            6 => "SERVER_DEVICE_BUSY",
            8 => "MEMORY_PARITY_ERROR",
            10 => "GATEWAY_PATH_UNAVAILABLE",
            11 => "GATEWAY_TARGET_DEVICE_FAILED_TO_RESPOND",
            _ => "UNKNOWN"
        };
    }

    private static async Task ProbeReadCoilsAsync(IModbusClient client, ushort startAddress, ushort count)
        => _ = await client.ReadCoilsAsync(startAddress, count);

    private static async Task ProbeReadDiscreteAsync(IModbusClient client, ushort startAddress, ushort count)
        => _ = await client.ReadDiscreteInputsAsync(startAddress, count);

    private static async Task ProbeReadHoldingAsync(IModbusClient client, ushort startAddress, ushort count)
        => _ = await client.ReadHoldingRegistersAsync(startAddress, count);

    private static async Task ProbeReadInputAsync(IModbusClient client, ushort startAddress, ushort count)
        => _ = await client.ReadInputRegistersAsync(startAddress, count);

    private static async Task<string[]> ReadCoilsAsStringsAsync(IModbusClient client, ushort startAddress, ushort count)
    {
        var values = await client.ReadCoilsAsync(startAddress, count);
        return values.Select(value => value ? "1" : "0").ToArray();
    }

    private static async Task<string[]> ReadDiscreteAsStringsAsync(IModbusClient client, ushort startAddress, ushort count)
    {
        var values = await client.ReadDiscreteInputsAsync(startAddress, count);
        return values.Select(value => value ? "1" : "0").ToArray();
    }

    private static async Task<string[]> ReadHoldingAsStringsAsync(IModbusClient client, ushort startAddress, ushort count)
    {
        var values = await client.ReadHoldingRegistersAsync(startAddress, count);
        return values.Select(value => value.ToString(CultureInfo.InvariantCulture)).ToArray();
    }

    private static async Task<string[]> ReadInputAsStringsAsync(IModbusClient client, ushort startAddress, ushort count)
    {
        var values = await client.ReadInputRegistersAsync(startAddress, count);
        return values.Select(value => value.ToString(CultureInfo.InvariantCulture)).ToArray();
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
            return changed.Length == 0
                ? new[] { "(no changes)" }
                : changed.Select(x => x.line);
        }

        var changedIndexes = changed.Select(x => x.i).ToHashSet();
        return current.Select((line, i) => changedIndexes.Contains(i) ? $"* {line}" : line);
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

        for (var i = 0; i < values.Length; i++)
        {
            var valueText = output.ShowBool ? values[i].ToString() : (values[i] ? "1" : "0");
            if (output.Quiet)
            {
                Console.WriteLine(valueText);
            }
            else
            {
                Console.WriteLine($"{startAddress + i}: {valueText}");
            }
        }
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

        for (var i = 0; i < values.Length; i++)
        {
            if (output.Quiet)
            {
                Console.WriteLine(values[i]);
            }
            else
            {
                Console.WriteLine($"{startAddress + i}: {values[i]}");
            }
        }
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

    private static readonly FunctionCodeSpec[] FunctionCodeSpecs =
    {
        new("FC01", "bits", ProtocolLimits.MaxBitsPerRead, ProbeReadCoilsAsync, ReadCoilsAsStringsAsync),
        new("FC02", "bits", ProtocolLimits.MaxBitsPerRead, ProbeReadDiscreteAsync, ReadDiscreteAsStringsAsync),
        new("FC03", "registers", ProtocolLimits.MaxRegistersPerRead, ProbeReadHoldingAsync, ReadHoldingAsStringsAsync),
        new("FC04", "registers", ProtocolLimits.MaxRegistersPerRead, ProbeReadInputAsync, ReadInputAsStringsAsync)
    };

    private const int ProbeMaxAttempts = 3;

    private sealed record FunctionCodeSpec(
        string CodeLabel,
        string CountUnitLabel,
        int MaxReadCount,
        Func<IModbusClient, ushort, ushort, Task> ProbeReadAsync,
        Func<IModbusClient, ushort, ushort, Task<string[]>> ReadValuesAsync);

    private sealed record FunctionCodeSpace(FunctionCodeSpec Spec, int MaxAddress, string? BoundaryReason);

    private sealed record FunctionCodeCapture(FunctionCodeSpace Space, string[] Values, bool[] VolatileMask);

    private sealed record ProbeReadResult(int StartAddress, int Count, bool Success, string? FailureReason);

    private sealed record ProbeFailure(CliErrorCategory Category, string Message);

    private sealed class ScanState
    {
        public ScanState(FunctionCodeSpace space, string[] previousValues)
        {
            Space = space;
            PreviousValues = previousValues;
        }

        public FunctionCodeSpace Space { get; }

        public string[] PreviousValues { get; set; }

        public HashSet<int> ChangedEver { get; } = new();
    }

    private static class ProtocolLimits
    {
        public const int MaxAddress = ushort.MaxValue;
        public const int InitialProbeWindow = 256;
        public const int MaxBitsPerRead = 2000;
        public const int MaxRegistersPerRead = 125;
    }

    private enum CliErrorCategory
    {
        Parameter,
        SerialTransport,
        ModbusTcpTransport,
        RtuTcpTransport,
        Transport,
        Gateway,
        ModbusDevice,
        ProtocolFrame,
        Runtime
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
