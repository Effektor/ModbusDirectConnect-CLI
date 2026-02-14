using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Client;

public static class ModbusClientFactory
{
    public static IModbusClient CreateClient(ResolvedConnection connection)
    {
        return ModbusDirektClient.Create(connection);
    }

    public static IModbusClient CreateClient(string host, int port, byte slaveId, int timeout, string protocol)
    {
        var connection = EndpointResolver.Resolve(new ConnectionOptions(
            Target: host,
            Host: host,
            Port: port,
            SlaveId: slaveId,
            Timeout: timeout,
            Retries: 0,
            Protocol: protocol,
            SerialPort: null,
            SerialBaud: 9600,
            SerialDataBits: 8,
            SerialParity: "none",
            SerialStopBits: "one"));

        return CreateClient(connection);
    }
}
