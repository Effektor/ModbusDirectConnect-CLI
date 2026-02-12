using ModbusDirectConnect.CLI.Infrastructure;

namespace ModbusDirectConnect.CLI.Tests;

public class ArgumentNormalizerTests
{
    [Fact]
    public void Normalize_InjectsTargetForFirstTokenWhenNotCommand()
    {
        var args = new[] { "COM3", "read", "coils", "0", "8" };

        var normalized = ArgumentNormalizer.Normalize(args);

        Assert.Equal(new[] { "--target", "COM3", "read", "coils", "0", "8" }, normalized);
    }

    [Fact]
    public void Normalize_ConvertsVerboseShorthandAndCapsAtThree()
    {
        var args = new[] { "-vv", "-v", "read", "coils", "0", "8" };

        var normalized = ArgumentNormalizer.Normalize(args);

        Assert.Equal(new[] { "read", "coils", "0", "8", "--verbosity-level", "3" }, normalized);
    }
}
