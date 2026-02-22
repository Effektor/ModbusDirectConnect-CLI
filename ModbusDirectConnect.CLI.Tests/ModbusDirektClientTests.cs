using System.Reflection;
using ModbusDirectConnect.CLI.Client;
using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Tests;

public class ModbusDirektClientTests
{
    [Fact]
    public void Create_SerialRtu_CreatesSerialChannel_AndSetsRetries()
    {
        var connection = new ResolvedConnection(
            Transport: TransportKind.RtuSerial,
            Host: null,
            Port: 502,
            SerialPort: "/dev/ttyUSB0",
            SlaveId: 1,
            Timeout: 1000,
            Retries: 2,
            SerialBaud: 19200,
            SerialDataBits: 8,
            SerialParity: "N",
            SerialStopBits: "1");

        using var client = ModbusDirektClient.Create(connection);

        var inner = GetInnerModbusClient(client);
        Assert.Equal(2, (int)GetPublicProperty(inner, "NumberOfRetries"));

        var channel = GetPublicProperty(inner, "Channel");
        Assert.Equal("ModbusDirekt.Modbus.Channel.Serial.ModbusSerialChannel", channel.GetType().FullName);
    }

    [Fact]
    public void Create_Tcp_SetsRetries()
    {
        var connection = new ResolvedConnection(
            Transport: TransportKind.Tcp,
            Host: "127.0.0.1",
            Port: 502,
            SerialPort: null,
            SlaveId: 1,
            Timeout: 1000,
            Retries: 3,
            SerialBaud: 19200,
            SerialDataBits: 8,
            SerialParity: "N",
            SerialStopBits: "1");

        using var client = ModbusDirektClient.Create(connection);
        var inner = GetInnerModbusClient(client);
        Assert.Equal(3, (int)GetPublicProperty(inner, "NumberOfRetries"));
    }

    [Theory]
    [InlineData("X")]
    [InlineData("mark")]
    public void Create_SerialRtu_RejectsInvalidParity(string parity)
    {
        var connection = new ResolvedConnection(
            Transport: TransportKind.RtuSerial,
            Host: null,
            Port: 502,
            SerialPort: "COM3",
            SlaveId: 1,
            Timeout: 1000,
            Retries: 0,
            SerialBaud: 19200,
            SerialDataBits: 8,
            SerialParity: parity,
            SerialStopBits: "1");

        var ex = Assert.Throws<ArgumentException>(() => ModbusDirektClient.Create(connection));
        Assert.Contains("Invalid --parity", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Create_SerialRtu_RequiresBaud()
    {
        var connection = new ResolvedConnection(
            Transport: TransportKind.RtuSerial,
            Host: null,
            Port: 502,
            SerialPort: "COM3",
            SlaveId: 1,
            Timeout: 1000,
            Retries: 0,
            SerialBaud: null,
            SerialDataBits: 8,
            SerialParity: "N",
            SerialStopBits: "1");

        var ex = Assert.Throws<ArgumentException>(() => ModbusDirektClient.Create(connection));
        Assert.Contains("--baud", ex.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1.5")]
    [InlineData("three")]
    public void Create_SerialRtu_RejectsInvalidStopBits(string stopBits)
    {
        var connection = new ResolvedConnection(
            Transport: TransportKind.RtuSerial,
            Host: null,
            Port: 502,
            SerialPort: "COM3",
            SlaveId: 1,
            Timeout: 1000,
            Retries: 0,
            SerialBaud: 19200,
            SerialDataBits: 8,
            SerialParity: "N",
            SerialStopBits: stopBits);

        var ex = Assert.Throws<ArgumentException>(() => ModbusDirektClient.Create(connection));
        Assert.Contains("Invalid --stopbits", ex.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    public void Create_SerialRtu_RejectsInvalidDataBits(int dataBits)
    {
        var connection = new ResolvedConnection(
            Transport: TransportKind.RtuSerial,
            Host: null,
            Port: 502,
            SerialPort: "COM3",
            SlaveId: 1,
            Timeout: 1000,
            Retries: 0,
            SerialBaud: 19200,
            SerialDataBits: dataBits,
            SerialParity: "N",
            SerialStopBits: "1");

        var ex = Assert.Throws<ArgumentException>(() => ModbusDirektClient.Create(connection));
        Assert.Contains("Invalid --databits", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateInvalidReadResponseException_IncludesOperationName()
    {
        var ex = ModbusDirektClient.CreateInvalidReadResponseException("holding registers");

        Assert.Contains("holding registers", ex.Message, StringComparison.Ordinal);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void CreateInvalidReadResponseException_RetainsInnerException()
    {
        var inner = new NullReferenceException("simulated library null");
        var ex = ModbusDirektClient.CreateInvalidReadResponseException("holding registers", inner);

        Assert.Same(inner, ex.InnerException);
    }

    private static object GetInnerModbusClient(ModbusDirektClient client)
    {
        var field = typeof(ModbusDirektClient).GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        var inner = field!.GetValue(client);
        Assert.NotNull(inner);
        return inner!;
    }

    private static object GetPublicProperty(object instance, string propertyName)
    {
        var prop = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(prop);
        var value = prop!.GetValue(instance);
        Assert.NotNull(value);
        return value!;
    }
}
