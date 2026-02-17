using System.Linq;
using System.Reflection;
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
    [InlineData("1:")]
    [InlineData("1-")]
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

    [Theory]
    [InlineData("65535:2", (ushort)1)]
    [InlineData("65535", (ushort)2)]
    public void ParseReadRanges_Overflow_ThrowsArgumentException(string spec, ushort fallbackCount)
    {
        Assert.Throws<ArgumentException>(() => FlatCommandMode.ParseReadRanges(spec, fallbackCount));
    }

    [Fact]
    public void ParseReadRanges_HexAndBinaryTokens_AreParsed()
    {
        var ranges = FlatCommandMode.ParseReadRanges("0x10:2,0b10010", 1);
        var actual = ranges.Select(r => (r.Address, r.Count)).ToArray();

        Assert.Equal(new[] { ((ushort)16, (ushort)3) }, actual);
    }

    [Fact]
    public void ParseReferenceRanges_SameReferenceType_ReturnsZeroBasedRanges()
    {
        var result = InvokeParseReferenceRanges("40001:2,40005", 1);

        Assert.Collection(result,
            item =>
            {
                Assert.Equal("HoldingRegister", GetReferenceTypeName(item));
                Assert.Equal((ushort)0, GetReferenceAddress(item));
                Assert.Equal((ushort)2, GetReferenceCount(item));
            },
            item =>
            {
                Assert.Equal("HoldingRegister", GetReferenceTypeName(item));
                Assert.Equal((ushort)4, GetReferenceAddress(item));
                Assert.Equal((ushort)1, GetReferenceCount(item));
            });
    }

    [Fact]
    public void ParseReferenceRanges_MixedReferenceTypes_ThrowsArgumentException()
    {
        var ex = Assert.Throws<TargetInvocationException>(() => InvokeParseReferenceRanges("40001,30001", 1));
        var inner = Assert.IsType<ArgumentException>(ex.InnerException);

        Assert.Contains("same Modicon reference type", inner.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ParseReferenceRanges_SingleRangeSpansReferenceTypes_ThrowsArgumentException()
    {
        var ex = Assert.Throws<TargetInvocationException>(() => InvokeParseReferenceRanges("9998:4", 1));
        var inner = Assert.IsType<ArgumentException>(ex.InnerException);

        Assert.Contains("must not span multiple Modicon reference types", inner.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ParseReferenceRanges_UnsupportedReference_ThrowsArgumentException()
    {
        var ex = Assert.Throws<TargetInvocationException>(() => InvokeParseReferenceRanges("50000", 1));
        var inner = Assert.IsType<ArgumentException>(ex.InnerException);

        Assert.Contains("Unsupported reference range", inner.Message, StringComparison.Ordinal);
    }

    public static IEnumerable<object[]> RangeData()
    {
        yield return new object[] { "1", (ushort)3, new ushort[] { 1, 3 } };
        yield return new object[] { "1-3,2,4-5", (ushort)1, new ushort[] { 1, 5 } };
        yield return new object[] { "5,10,6-8", (ushort)1, new ushort[] { 5, 4, 10, 1 } };
        yield return new object[] { "1:3,8,10-12", (ushort)1, new ushort[] { 1, 3, 8, 1, 10, 3 } };
    }

    private static IReadOnlyList<object> InvokeParseReferenceRanges(string spec, ushort fallbackCount)
    {
        var method = typeof(FlatCommandMode).GetMethod("ParseReferenceRanges", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var result = method!.Invoke(null, new object[] { spec, fallbackCount });
        var enumerable = Assert.IsAssignableFrom<System.Collections.IEnumerable>(result);

        return enumerable.Cast<object>().ToArray();
    }

    private static string GetReferenceTypeName(object request)
    {
        var property = request.GetType().GetProperty("ReferenceType", BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);
        var value = property!.GetValue(request);
        Assert.NotNull(value);

        return value!.ToString()!;
    }

    private static ushort GetReferenceAddress(object request)
    {
        var property = request.GetType().GetProperty("Address", BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);
        var value = property!.GetValue(request);

        return Assert.IsType<ushort>(value);
    }

    private static ushort GetReferenceCount(object request)
    {
        var property = request.GetType().GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);
        var value = property!.GetValue(request);

        return Assert.IsType<ushort>(value);
    }
}
