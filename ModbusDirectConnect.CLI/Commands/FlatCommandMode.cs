using System.CommandLine;
using System.CommandLine.Invocation;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
                    var ranges = ParseReadRanges(readCoilSpec, readCountDefault);
                    await ExecuteReadLoop(ranges, range => client.ReadCoilsAsync(range.Address, range.Count), (range, values) => RenderBoolValues(range.Address, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readDiscreteOption) is { } readDiscreteSpec)
                {
                    var ranges = ParseReadRanges(readDiscreteSpec, readCountDefault);
                    await ExecuteReadLoop(ranges, range => client.ReadDiscreteInputsAsync(range.Address, range.Count), (range, values) => RenderBoolValues(range.Address, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readHoldingOption) is { } readHoldingSpec)
                {
                    var ranges = ParseReadRanges(readHoldingSpec, readCountDefault);
                    await ExecuteReadLoop(ranges, range => client.ReadHoldingRegistersAsync(range.Address, range.Count), (range, values) => RenderRegisterValues(range.Address, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readInputRegOption) is { } readInputSpec)
                {
                    var ranges = ParseReadRanges(readInputSpec, readCountDefault);
                    await ExecuteReadLoop(ranges, range => client.ReadInputRegistersAsync(range.Address, range.Count), (range, values) => RenderRegisterValues(range.Address, values, output), output);
                    return;
                }

                if (context.ParseResult.GetValueForOption(readRefOption) is { } readRefSpec)
                {
                    var referenceRanges = ParseReferenceRanges(readRefSpec, readCountDefault);
                    if (referenceRanges.Count == 0)
                    {
                        throw new ArgumentException("Reference specification did not define any ranges.");
                    }

                    switch (referenceRanges[0].ReferenceType)
                    {
                        case ReferenceType.Coil:
                            await ExecuteReadLoop(referenceRanges, range => client.ReadCoilsAsync(range.Address, range.Count), (range, values) => RenderBoolValues(range.Address, values, output), output);
                            break;
                        case ReferenceType.Discrete:
                            await ExecuteReadLoop(referenceRanges, range => client.ReadDiscreteInputsAsync(range.Address, range.Count), (range, values) => RenderBoolValues(range.Address, values, output), output);
                            break;
                        case ReferenceType.InputRegister:
                            await ExecuteReadLoop(referenceRanges, range => client.ReadInputRegistersAsync(range.Address, range.Count), (range, values) => RenderRegisterValues(range.Address, values, output), output);
                            break;
                        case ReferenceType.HoldingRegister:
                            await ExecuteReadLoop(referenceRanges, range => client.ReadHoldingRegistersAsync(range.Address, range.Count), (range, values) => RenderRegisterValues(range.Address, values, output), output);
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

    internal static IReadOnlyList<ReadRange> ParseReadRanges(string spec, ushort fallbackCount)
    {
        if (string.IsNullOrWhiteSpace(spec))
        {
            throw new ArgumentException("SPEC is required.", nameof(spec));
        }

        if (fallbackCount == 0)
        {
            throw new ArgumentException("--count must be at least 1.", nameof(fallbackCount));
        }

        var tokens = spec.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            throw new ArgumentException($"Invalid specification '{spec}'.", nameof(spec));
        }

        var addresses = new SortedSet<ushort>();
        foreach (var token in tokens)
        {
            foreach (var address in ExpandRangeToken(token, fallbackCount))
            {
                addresses.Add(address);
            }
        }

        if (addresses.Count == 0)
        {
            throw new ArgumentException($"Invalid specification '{spec}'.", nameof(spec));
        }

        var ranges = new List<ReadRange>();
        ushort? rangeStart = null;
        ushort rangeEnd = 0;

        foreach (var address in addresses)
        {
            if (rangeStart is null)
            {
                rangeStart = address;
                rangeEnd = address;
                continue;
            }

            var nextExpected = (uint)rangeEnd + 1;
            if (nextExpected <= ushort.MaxValue && address == nextExpected)
            {
                rangeEnd = address;
                continue;
            }

            ranges.Add(new ReadRange(rangeStart.Value, (ushort)(rangeEnd - rangeStart.Value + 1)));
            rangeStart = address;
            rangeEnd = address;
        }

        if (rangeStart is not null)
        {
            ranges.Add(new ReadRange(rangeStart.Value, (ushort)(rangeEnd - rangeStart.Value + 1)));
        }

        return ranges;
    }

    private static IReadOnlyList<ReferenceReadRequest> ParseReferenceRanges(string spec, ushort fallbackCount)
    {
        var ranges = ParseReadRanges(spec, fallbackCount);
        if (ranges.Count == 0)
        {
            return ranges;
        }

        ReferenceType? detectedType = null;
        var resolved = new List<ReferenceReadRequest>(ranges.Count);

        foreach (var range in ranges)
        {
            var startReference = range.Address;
            var endReference = (uint)range.Address + range.Count - 1;

            var referenceType = DetermineReferenceType(startReference);
            var endType = DetermineReferenceType((ushort)endReference);
            if (referenceType != endType)
            {
                throw new ArgumentException("Reference ranges must not span multiple Modicon reference types.");
            }

            if (detectedType is null)
            {
                detectedType = referenceType;
            }
            else if (detectedType != referenceType)
            {
                throw new ArgumentException("All reference ranges must target the same Modicon reference type.");
            }

            var zeroBasedAddress = referenceType switch
            {
                ReferenceType.Coil => (ushort)(startReference - 1),
                ReferenceType.Discrete => (ushort)(startReference - 10001),
                ReferenceType.InputRegister => (ushort)(startReference - 30001),
                ReferenceType.HoldingRegister => (ushort)(startReference - 40001),
                _ => throw new InvalidOperationException($"Unsupported reference type {referenceType}.")
            };

            resolved.Add(new ReferenceReadRequest(referenceType, zeroBasedAddress, range.Count));
        }

        return resolved;
    }

    private static IEnumerable<ushort> ExpandRangeToken(string token, ushort fallbackCount)
    {
        var trimmed = token.Trim();
        if (trimmed.Length == 0)
        {
            yield break;
        }

        if (trimmed.Contains(':'))
        {
            var parts = trimmed.Split(new[] { ':' }, 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new ArgumentException($"Invalid range token '{token}'.", nameof(token));
            }

            var start = ParseWord(parts[0]);
            var count = ParseWord(parts[1]);
            if (count == 0)
            {
                throw new ArgumentException($"Range '{token}' must specify a count of at least 1.", nameof(token));
            }

            var end = (uint)start + count - 1;
            if (end > ushort.MaxValue)
            {
                throw new ArgumentException($"Range '{token}' exceeds the address space.", nameof(token));
            }

            for (uint value = start; value <= end; value++)
            {
                yield return (ushort)value;
            }

            yield break;
        }

        if (trimmed.Contains('-'))
        {
            var parts = trimmed.Split(new[] { '-' }, 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new ArgumentException($"Invalid range token '{token}'.", nameof(token));
            }

            var start = ParseWord(parts[0]);
            var end = ParseWord(parts[1]);
            if (end < start)
            {
                throw new ArgumentException($"Range '{token}' end must not be less than its start.", nameof(token));
            }

            for (uint value = start; value <= end; value++)
            {
                yield return (ushort)value;
            }

            yield break;
        }

        var single = ParseWord(trimmed);
        var endInclusive = (uint)single + fallbackCount - 1;
        if (endInclusive > ushort.MaxValue)
        {
            throw new ArgumentException($"Range '{token}' exceeds the address space.", nameof(token));
        }

        for (uint value = single; value <= endInclusive; value++)
        {
            yield return (ushort)value;
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

    private static async Task ExecuteReadLoop<TRange, TResult>(
        IReadOnlyList<TRange> ranges,
        Func<TRange, Task<TResult>> readFunc,
        Action<TRange, TResult> render,
        OutputOptions output)
    {
        if (ranges.Count == 0)
        {
            return;
        }

        var interval = output.MonitorIntervalSeconds ?? output.WatchIntervalSeconds;
        if (interval is null)
        {
            foreach (var range in ranges)
            {
                var result = await readFunc(range);
                render(range, result);
            }

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
            if (output.MonitorIntervalSeconds is not null)
            {
                Console.Clear();
            }

            var originalOut = Console.Out;
            using var buffer = new StringWriter();
            Console.SetOut(buffer);

            foreach (var range in ranges)
            {
                var result = await readFunc(range);
                render(range, result);
            }

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

    private static ReferenceType DetermineReferenceType(ushort reference)
    {
        if (reference >= 1 && reference <= 9999)
        {
            return ReferenceType.Coil;
        }

        if (reference >= 10001 && reference <= 19999)
        {
            return ReferenceType.Discrete;
        }

        if (reference >= 30001 && reference <= 39999)
        {
            return ReferenceType.InputRegister;
        }

        if (reference >= 40001 && reference <= 49999)
        {
            return ReferenceType.HoldingRegister;
        }

        throw new ArgumentException($"Unsupported reference range: {reference}");
    }

    private enum ReferenceType
    {
        Coil,
        Discrete,
        InputRegister,
        HoldingRegister
    }

    private sealed record ReferenceReadRequest(ReferenceType ReferenceType, ushort Address, ushort Count);

    internal sealed record ReadRange(ushort Address, ushort Count);

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
