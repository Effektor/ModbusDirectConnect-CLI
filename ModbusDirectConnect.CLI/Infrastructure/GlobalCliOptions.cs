using System.CommandLine;

namespace ModbusDirectConnect.CLI.Infrastructure;

public sealed class GlobalCliOptions
{
    public required Option<string?> Target { get; init; }
    public required Option<string?> Host { get; init; }
    public required Option<int> Port { get; init; }
    public required Option<byte> SlaveId { get; init; }
    public required Option<double> TimeoutSeconds { get; init; }
    public required Option<int> Retries { get; init; }
    public required Option<string?> Protocol { get; init; }
    public required Option<string?> SerialPort { get; init; }
    public required Option<int> SerialBaud { get; init; }
    public required Option<int> SerialDataBits { get; init; }
    public required Option<string> SerialParity { get; init; }
    public required Option<string> SerialStopBits { get; init; }
    public required Option<int> VerbosityLevel { get; init; }
}
