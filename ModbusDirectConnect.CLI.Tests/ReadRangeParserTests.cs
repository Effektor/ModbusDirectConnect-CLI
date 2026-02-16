using System.Linq;
using ModbusDirectConnect.CLI.Commands;
using Xunit;

namespace ModbusDirectConnect.CLI.Tests;

public class ReadRangeParserTests
{
    [Theory]
    [MemberData(nameof(RangeData))]
    public void ParseReadRanges_MergesAndSortsRanges(string spec, ushort fallbackCount, ushort[] expectedPairs)
    {
        var ranges = FlatCommandMode.ParseReadRanges(spec, fallbackCount);
        var actual = ranges.Select(r => (r.Address, r.Count)).ToArray();
        var expected = Enumerable.Range(0, expectedPairs.Length / 2)
            .Select(i => (expectedPairs[i * 2], expectedPairs[i * 2 + 1]))
            .ToArray();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("5-3")]
    [InlineData("")]
    [InlineData(" ")]
    public void ParseReadRanges_InvalidSyntax_ThrowsArgumentException(string spec)
    {
        Assert.Throws<ArgumentException>(() => FlatCommandMode.ParseReadRanges(spec, 1));
    }

    [Fact]
    public void ParseReadRanges_FallbackCountZero_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => FlatCommandMode.ParseReadRanges("1", 0));
    }

    public static IEnumerable<object[]> RangeData()
    {
        yield return new object[] { "1", (ushort)3, new ushort[] { 1, 3 } };
        yield return new object[] { "1-3,2,4-5", (ushort)1, new ushort[] { 1, 5 } };
        yield return new object[] { "5,10,6-8", (ushort)1, new ushort[] { 5, 4, 10, 1 } };
        yield return new object[] { "1:3,8,10-12", (ushort)1, new ushort[] { 1, 3, 8, 1, 10, 3 } };
    }
}
