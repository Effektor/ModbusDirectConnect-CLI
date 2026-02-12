namespace ModbusDirectConnect.CLI.Client;

/// <summary>
/// Interface for Modbus client operations
/// </summary>
public interface IModbusClient : IDisposable
{
    /// <summary>
    /// Read coils (function code 1)
    /// </summary>
    Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort count);

    /// <summary>
    /// Read discrete inputs (function code 2)
    /// </summary>
    Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort count);

    /// <summary>
    /// Read holding registers (function code 3)
    /// </summary>
    Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort count);

    /// <summary>
    /// Read input registers (function code 4)
    /// </summary>
    Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort count);

    /// <summary>
    /// Write single coil (function code 5)
    /// </summary>
    Task WriteSingleCoilAsync(ushort address, bool value);

    /// <summary>
    /// Write single register (function code 6)
    /// </summary>
    Task WriteSingleRegisterAsync(ushort address, ushort value);

    /// <summary>
    /// Write multiple coils (function code 15)
    /// </summary>
    Task WriteMultipleCoilsAsync(ushort startAddress, bool[] values);

    /// <summary>
    /// Write multiple registers (function code 16)
    /// </summary>
    Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] values);
}
