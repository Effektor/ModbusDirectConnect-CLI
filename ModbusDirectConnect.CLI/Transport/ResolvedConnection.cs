namespace ModbusDirectConnect.CLI.Transport;

public sealed record ResolvedConnection(
    TransportKind Transport,
    string? Host,
    int Port,
    string? SerialPort,
    byte SlaveId,
    int Timeout,
    int SerialBaud,
    int SerialDataBits,
    string SerialParity,
    string SerialStopBits)
{
    public string DisplayTarget => Transport switch
    {
        TransportKind.RtuSerial => SerialPort ?? "(unknown-serial-port)",
        TransportKind.RtuTcp => $"rtu-tcp://{Host}:{Port}",
        _ => $"tcp://{Host}:{Port}"
    };
}
