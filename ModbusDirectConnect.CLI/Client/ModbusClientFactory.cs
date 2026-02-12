namespace ModbusDirectConnect.CLI.Client;

/// <summary>
/// Factory for creating Modbus client instances
/// </summary>
public static class ModbusClientFactory
{
    public static IModbusClient CreateClient(string host, int port, byte slaveId, int timeout, string protocol)
    {
        return protocol.ToLowerInvariant() switch
        {
            "tcp" => new ModbusTcpClient(host, port, slaveId, timeout),
            "rtu" => throw new NotImplementedException("RTU protocol support will be added with the ModbusDirectConnect library integration"),
            _ => throw new ArgumentException($"Unknown protocol: {protocol}. Supported protocols: tcp, rtu")
        };
    }
}
