using ModbusDirekt;
using ModbusDirekt.Modbus.Channel;
using ModbusDirekt.Modbus.Channel.TCP;
using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Client;

public sealed class ModbusDirektClient : IModbusClient
{
    private readonly ModbusClient _client;

    private ModbusDirektClient(ModbusClient client)
    {
        _client = client;
    }

    public static ModbusDirektClient Create(ResolvedConnection connection)
    {
        ModbusChannel channel = connection.Transport switch
        {
            TransportKind.Tcp => CreateTcpChannel(connection),
            TransportKind.RtuTcp => new RtuTCPChannel(connection.Host!, connection.Port, connection.SlaveId),
            TransportKind.RtuSerial => throw new NotSupportedException("RTU serial transport is not currently available in ModbusDirectConnect 1.1.1 (no public serial channel API in the net8.0 package)."),
            _ => throw new ArgumentOutOfRangeException(nameof(connection.Transport), connection.Transport, "Unsupported transport")
        };

        return new ModbusDirektClient(new ModbusClient(channel));
    }

    public Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort count)
    {
        var coils = _client.ReadCoils(startAddress, count);
        var result = new bool[count];

        for (var i = 0; i < count; i++)
        {
            result[i] = coils[i];
        }

        return Task.FromResult(result);
    }

    public Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort count)
    {
        var inputs = _client.ReadDiscreteInputs(startAddress, count);
        var result = new bool[count];

        for (var i = 0; i < count; i++)
        {
            result[i] = inputs[i];
        }

        return Task.FromResult(result);
    }

    public Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort count)
    {
        var registers = _client.ReadHoldingRegisters(startAddress, count);
        var result = new ushort[count];

        for (var i = 0; i < count; i++)
        {
            result[i] = unchecked((ushort)registers[i]);
        }

        return Task.FromResult(result);
    }

    public Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort count)
    {
        var inputRegisters = _client.ReadInputRegisters(startAddress, count);
        var bytes = inputRegisters.Data;
        var result = new ushort[count];

        for (var i = 0; i < count; i++)
        {
            var index = i * 2;
            if (index + 1 >= bytes.Length)
            {
                break;
            }

            result[i] = (ushort)((bytes[index] << 8) | bytes[index + 1]);
        }

        return Task.FromResult(result);
    }

    public Task WriteSingleCoilAsync(ushort address, bool value)
    {
        throw new NotSupportedException("Writing coils is not exposed by ModbusDirectConnect 1.1.1 public API.");
    }

    public Task WriteSingleRegisterAsync(ushort address, ushort value)
    {
        _client.WriteHoldingRegister(address, value);
        return Task.CompletedTask;
    }

    public Task WriteMultipleCoilsAsync(ushort startAddress, bool[] values)
    {
        throw new NotSupportedException("Writing multiple coils is not exposed by ModbusDirectConnect 1.1.1 public API.");
    }

    public Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            var address = (ushort)(startAddress + i);
            _client.WriteHoldingRegister(address, values[i]);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _client.Channel.Disconnect();
    }

    private static ModbusTCPChannel CreateTcpChannel(ResolvedConnection connection)
    {
        return new ModbusTCPChannel(connection.Host!, connection.SlaveId, connection.Port)
        {
            ConnectionTimeout = connection.Timeout
        };
    }
}
