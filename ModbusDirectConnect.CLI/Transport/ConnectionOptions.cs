namespace ModbusDirectConnect.CLI.Transport;

public sealed record ConnectionOptions(
    string? Target,
    string? Host,
    int Port,
    byte SlaveId,
    int Timeout,
    string? Protocol,
    string? SerialPort,
    int SerialBaud,
    int SerialDataBits,
    string SerialParity,
    string SerialStopBits);
