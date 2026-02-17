using ModbusDirekt;
using ModbusDirekt.Modbus.Channel;
using ModbusDirekt.Modbus.Channel.Serial;
using ModbusDirekt.Modbus.Channel.TCP;
using ModbusDirectConnect.CLI.Transport;
using System.IO.Ports;

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
            TransportKind.RtuSerial => CreateSerialChannel(connection),
            _ => throw new ArgumentOutOfRangeException(nameof(connection.Transport), connection.Transport, "Unsupported transport")
        };

        var client = new ModbusClient(channel)
        {
            NumberOfRetries = Math.Max(0, connection.Retries)
        };

        return new ModbusDirektClient(client);
    }

    public Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort count)
    {
        try
        {
            var coils = _client.ReadCoils(startAddress, count);
            if (coils is null)
            {
                throw CreateInvalidReadResponseException("coils");
            }

            var result = new bool[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = coils[i];
            }

            return Task.FromResult(result);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw CreateInvalidReadResponseException("coils", ex);
        }
    }

    public Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort count)
    {
        try
        {
            var inputs = _client.ReadDiscreteInputs(startAddress, count);
            if (inputs is null)
            {
                throw CreateInvalidReadResponseException("discrete inputs");
            }

            var result = new bool[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = inputs[i];
            }

            return Task.FromResult(result);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw CreateInvalidReadResponseException("discrete inputs", ex);
        }
    }

    public Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort count)
    {
        try
        {
            var registers = _client.ReadHoldingRegisters(startAddress, count);
            if (registers?.Data is null)
            {
                throw CreateInvalidReadResponseException("holding registers");
            }

            var bytes = registers.Data;
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
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw CreateInvalidReadResponseException("holding registers", ex);
        }
    }

    public Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort count)
    {
        try
        {
            var inputRegisters = _client.ReadInputRegisters(startAddress, count);
            if (inputRegisters?.Data is null)
            {
                throw CreateInvalidReadResponseException("input registers");
            }

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
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw CreateInvalidReadResponseException("input registers", ex);
        }
    }

    public Task WriteSingleCoilAsync(ushort address, bool value)
    {
        throw new NotSupportedException("Writing coils is not exposed by the ModbusDirectConnect public API.");
    }

    public Task WriteSingleRegisterAsync(ushort address, ushort value)
    {
        _client.WriteHoldingRegister(address, value);
        return Task.CompletedTask;
    }

    public Task WriteMultipleCoilsAsync(ushort startAddress, bool[] values)
    {
        throw new NotSupportedException("Writing multiple coils is not exposed by the ModbusDirectConnect public API.");
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

    private static ModbusSerialChannel CreateSerialChannel(ResolvedConnection connection)
    {
        if (string.IsNullOrWhiteSpace(connection.SerialPort))
        {
            throw new ArgumentException("Serial transport requires a serial port path or port name.");
        }

        if (connection.SerialBaud <= 0)
        {
            throw new ArgumentException($"Invalid --baud '{connection.SerialBaud}'. Baud rate must be a positive integer.");
        }

        if (connection.SerialDataBits is not (7 or 8))
        {
            throw new ArgumentException($"Invalid --databits '{connection.SerialDataBits}'. Supported values: 7, 8");
        }

        return new ModbusSerialChannel(
            portName: connection.SerialPort,
            unitIdentifier: connection.SlaveId,
            baudRate: connection.SerialBaud,
            parity: ParseParity(connection.SerialParity),
            dataBits: connection.SerialDataBits,
            stopBits: ParseStopBits(connection.SerialStopBits),
            readTimeout: connection.Timeout,
            writeTimeout: connection.Timeout,
            handshake: Handshake.None,
            portFlavor: ResolvePortFlavor(connection.SerialPort));
    }

    private static Parity ParseParity(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        return normalized switch
        {
            "n" or "none" => Parity.None,
            "e" or "even" => Parity.Even,
            "o" or "odd" => Parity.Odd,
            _ => throw new ArgumentException($"Invalid --parity '{value}'. Supported values: N, E, O")
        };
    }

    private static StopBits ParseStopBits(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        return normalized switch
        {
            "1" or "one" => StopBits.One,
            "2" or "two" => StopBits.Two,
            _ => throw new ArgumentException($"Invalid --stopbits '{value}'. Supported values: 1, 2")
        };
    }

    private static SerialPortFlavor ResolvePortFlavor(string portName)
    {
        if (portName.StartsWith("/dev/", StringComparison.OrdinalIgnoreCase))
        {
            return SerialPortFlavor.LinuxDevice;
        }

        if (portName.StartsWith(@"\\.\COM", StringComparison.OrdinalIgnoreCase) ||
            (portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase) &&
             portName.Length > 3 &&
             portName[3..].All(char.IsDigit)))
        {
            return SerialPortFlavor.WindowsCom;
        }

        return SerialPortFlavor.Auto;
    }

    internal static InvalidOperationException CreateInvalidReadResponseException(string operation, Exception? innerException = null)
    {
        return new InvalidOperationException(
            $"Read {operation} failed due to an invalid or incomplete response from the Modbus runtime.",
            innerException);
    }
}
