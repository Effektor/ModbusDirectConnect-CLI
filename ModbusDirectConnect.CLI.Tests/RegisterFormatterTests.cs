using ModbusDirectConnect.CLI.Output;

namespace ModbusDirectConnect.CLI.Tests;

public class RegisterFormatterTests
{
    [Fact]
    public void FormatRegisters_FormatsHexPerRegister()
    {
        var lines = RegisterFormatter.FormatRegisters(100, new ushort[] { 0x1234, 0xABCD }, RegisterFormat.Hex, bigEndian: true);

        Assert.Equal(new[] { "100: 0x1234", "101: 0xABCD" }, lines);
    }

    [Fact]
    public void FormatSingleLine_FormatsCStringUntilNullByte()
    {
        var registers = new ushort[] { 0x4865, 0x6C6C, 0x6F00, 0x5A5A };

        var line = RegisterFormatter.FormatSingleLine(registers, RegisterFormat.CString, bigEndian: true);

        Assert.Equal("Hello", line);
    }

    [Fact]
    public void ParseFormat_ThrowsForInvalidFormat()
    {
        Assert.Throws<ArgumentException>(() => RegisterFormatter.ParseFormat("json"));
    }
}
