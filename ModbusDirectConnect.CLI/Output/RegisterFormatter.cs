using System.Globalization;
using System.Text;

namespace ModbusDirectConnect.CLI.Output;

public static class RegisterFormatter
{
    public static RegisterFormat ParseFormat(string format)
    {
        return format.Trim().ToLowerInvariant() switch
        {
            "u16" => RegisterFormat.U16,
            "hex" => RegisterFormat.Hex,
            "ascii" => RegisterFormat.Ascii,
            "utf8" => RegisterFormat.Utf8,
            "cstring" => RegisterFormat.CString,
            _ => throw new ArgumentException($"Unknown register format '{format}'. Supported values: u16, hex, ascii, utf8, cstring")
        };
    }

    public static string[] FormatRegisters(ushort startAddress, ushort[] values, RegisterFormat format, bool bigEndian)
    {
        if (format == RegisterFormat.U16)
        {
            return values.Select((value, offset) => $"{startAddress + offset}: {value}").ToArray();
        }

        var bytes = ToBytes(values, bigEndian);

        return format switch
        {
            RegisterFormat.Hex => bytes
                .Chunk(2)
                .Select((chunk, offset) => $"{startAddress + offset}: 0x{BitConverter.ToString(chunk.ToArray()).Replace("-", string.Empty, StringComparison.Ordinal)}")
                .ToArray(),
            RegisterFormat.Ascii => new[] { $"{startAddress}: {Encoding.ASCII.GetString(bytes)}" },
            RegisterFormat.Utf8 => new[] { $"{startAddress}: {Encoding.UTF8.GetString(bytes)}" },
            RegisterFormat.CString => new[] { $"{startAddress}: {ReadCString(bytes)}" },
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported register format")
        };
    }

    public static string FormatSingleLine(ushort[] values, RegisterFormat format, bool bigEndian)
    {
        if (format == RegisterFormat.U16)
        {
            return string.Join(' ', values.Select(v => v.ToString(CultureInfo.InvariantCulture)));
        }

        var bytes = ToBytes(values, bigEndian);

        return format switch
        {
            RegisterFormat.Hex => BitConverter.ToString(bytes).Replace("-", string.Empty, StringComparison.Ordinal),
            RegisterFormat.Ascii => Encoding.ASCII.GetString(bytes),
            RegisterFormat.Utf8 => Encoding.UTF8.GetString(bytes),
            RegisterFormat.CString => ReadCString(bytes),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported register format")
        };
    }

    public static byte[] ToBytes(ushort[] values, bool bigEndian)
    {
        var bytes = new byte[values.Length * 2];

        for (var i = 0; i < values.Length; i++)
        {
            var register = values[i];
            var offset = i * 2;

            if (bigEndian)
            {
                bytes[offset] = (byte)(register >> 8);
                bytes[offset + 1] = (byte)(register & 0xFF);
            }
            else
            {
                bytes[offset] = (byte)(register & 0xFF);
                bytes[offset + 1] = (byte)(register >> 8);
            }
        }

        return bytes;
    }

    private static string ReadCString(byte[] bytes)
    {
        var nullIndex = Array.IndexOf(bytes, (byte)0);
        var length = nullIndex >= 0 ? nullIndex : bytes.Length;
        return Encoding.UTF8.GetString(bytes, 0, length);
    }
}
