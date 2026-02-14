using ModbusDirectConnect.CLI.Transport;

namespace ModbusDirectConnect.CLI.Tests;

public class EndpointResolverTests
{
    [Fact]
    public void Resolve_InfersSerialFromComPortTarget()
    {
        var resolved = EndpointResolver.Resolve(new ConnectionOptions(
            Target: "COM4",
            Host: null,
            Port: 502,
            SlaveId: 1,
            Timeout: 5000,
            Retries: 0,
            Protocol: null,
            SerialPort: null,
            SerialBaud: 9600,
            SerialDataBits: 8,
            SerialParity: "none",
            SerialStopBits: "one"));

        Assert.Equal(TransportKind.RtuSerial, resolved.Transport);
        Assert.Equal("COM4", resolved.SerialPort);
    }

    [Fact]
    public void Resolve_InfersTcpAndReadsPortFromTarget()
    {
        var resolved = EndpointResolver.Resolve(new ConnectionOptions(
            Target: "192.168.0.10:1502",
            Host: null,
            Port: 502,
            SlaveId: 2,
            Timeout: 3000,
            Retries: 0,
            Protocol: null,
            SerialPort: null,
            SerialBaud: 9600,
            SerialDataBits: 8,
            SerialParity: "none",
            SerialStopBits: "one"));

        Assert.Equal(TransportKind.Tcp, resolved.Transport);
        Assert.Equal("192.168.0.10", resolved.Host);
        Assert.Equal(1502, resolved.Port);
    }

    [Fact]
    public void Resolve_UsesExplicitProtocolOverInference()
    {
        var resolved = EndpointResolver.Resolve(new ConnectionOptions(
            Target: "tcp://10.0.0.4:502",
            Host: null,
            Port: 502,
            SlaveId: 1,
            Timeout: 5000,
            Retries: 0,
            Protocol: "rtu-tcp",
            SerialPort: null,
            SerialBaud: 9600,
            SerialDataBits: 8,
            SerialParity: "none",
            SerialStopBits: "one"));

        Assert.Equal(TransportKind.RtuTcp, resolved.Transport);
    }
}
