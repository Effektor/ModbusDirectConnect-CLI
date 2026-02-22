using System.Text.RegularExpressions;

namespace ModbusDirectConnect.CLI.Transport;

public static class EndpointResolver
{
    private static readonly Regex SerialPortPattern = new(@"^(?:(?:\\\\\.\\)?COM\d+|/dev/.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static ResolvedConnection Resolve(ConnectionOptions options)
    {
        var target = string.IsNullOrWhiteSpace(options.Target) ? null : options.Target!.Trim();
        var protocol = ResolveTransport(options.Protocol, target);

        var (targetHost, targetPort, targetSerial) = ParseTarget(target);

        return protocol switch
        {
            TransportKind.RtuSerial => ResolveSerial(options, targetSerial, target),
            TransportKind.RtuTcp => ResolveTcpLike(options, targetHost, targetPort, TransportKind.RtuTcp),
            _ => ResolveTcpLike(options, targetHost, targetPort, TransportKind.Tcp)
        };
    }

    private static ResolvedConnection ResolveSerial(ConnectionOptions options, string? targetSerial, string? target)
    {
        var serialPort = FirstNonEmpty(targetSerial, options.SerialPort, IsSerialPortLike(options.Host) ? options.Host : null);

        if (string.IsNullOrWhiteSpace(serialPort))
        {
            throw new ArgumentException("Serial transport requires a serial port target (e.g. COM3 or /dev/ttyUSB0) or --serial-port.");
        }

        if (!options.SerialBaud.HasValue)
        {
            throw new ArgumentException("Serial transport requires --baud to be provided explicitly.");
        }

        return new ResolvedConnection(
            Transport: TransportKind.RtuSerial,
            Host: null,
            Port: options.Port,
            SerialPort: serialPort,
            SlaveId: options.SlaveId,
            Timeout: options.Timeout,
            Retries: options.Retries,
            SerialBaud: options.SerialBaud,
            SerialDataBits: options.SerialDataBits,
            SerialParity: options.SerialParity,
            SerialStopBits: options.SerialStopBits);
    }

    private static ResolvedConnection ResolveTcpLike(ConnectionOptions options, string? targetHost, int? targetPort, TransportKind transport)
    {
        var host = FirstNonEmpty(targetHost, options.Host, "localhost")!;
        var port = targetPort ?? options.Port;

        return new ResolvedConnection(
            Transport: transport,
            Host: host,
            Port: port,
            SerialPort: null,
            SlaveId: options.SlaveId,
            Timeout: options.Timeout,
            Retries: options.Retries,
            SerialBaud: options.SerialBaud,
            SerialDataBits: options.SerialDataBits,
            SerialParity: options.SerialParity,
            SerialStopBits: options.SerialStopBits);
    }

    private static TransportKind ResolveTransport(string? protocol, string? target)
    {
        if (!string.IsNullOrWhiteSpace(protocol))
        {
            return ParseTransport(protocol!);
        }

        if (!string.IsNullOrWhiteSpace(target))
        {
            if (target.StartsWith("rtu-tcp://", StringComparison.OrdinalIgnoreCase))
            {
                return TransportKind.RtuTcp;
            }

            if (target.StartsWith("tcp://", StringComparison.OrdinalIgnoreCase))
            {
                return TransportKind.Tcp;
            }

            if (IsSerialPortLike(target))
            {
                return TransportKind.RtuSerial;
            }
        }

        return TransportKind.Tcp;
    }

    private static TransportKind ParseTransport(string protocol)
    {
        return protocol.Trim().ToLowerInvariant() switch
        {
            "tcp" => TransportKind.Tcp,
            "rtu-tcp" => TransportKind.RtuTcp,
            "rtu" => TransportKind.RtuSerial,
            "rtu-serial" => TransportKind.RtuSerial,
            _ => throw new ArgumentException($"Unknown protocol '{protocol}'. Supported values: tcp, rtu-tcp, rtu-serial")
        };
    }

    private static (string? Host, int? Port, string? SerialPort) ParseTarget(string? target)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            return (null, null, null);
        }

        if (IsSerialPortLike(target))
        {
            return (null, null, target);
        }

        if (Uri.TryCreate(target, UriKind.Absolute, out var absoluteUri) && !string.IsNullOrWhiteSpace(absoluteUri.Host))
        {
            var port = absoluteUri.IsDefaultPort ? (int?)null : absoluteUri.Port;
            return (absoluteUri.Host, port, null);
        }

        var hostPort = ParseHostPort(target);
        if (hostPort.HasValue)
        {
            return (hostPort.Value.Host, hostPort.Value.Port, null);
        }

        return (target, null, null);
    }

    private static (string Host, int Port)? ParseHostPort(string input)
    {
        var splitIndex = input.LastIndexOf(':');
        if (splitIndex <= 0 || splitIndex == input.Length - 1)
        {
            return null;
        }

        var host = input[..splitIndex];
        var portPart = input[(splitIndex + 1)..];

        if (!int.TryParse(portPart, out var port))
        {
            return null;
        }

        return (host, port);
    }

    private static bool IsSerialPortLike(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) && SerialPortPattern.IsMatch(value);
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
}
