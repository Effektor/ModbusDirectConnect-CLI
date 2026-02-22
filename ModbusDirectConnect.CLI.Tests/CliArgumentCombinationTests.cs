using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;
using ModbusDirectConnect.CLI;
using ModbusDirectConnect.CLI.Infrastructure;

namespace ModbusDirectConnect.CLI.Tests;

public class CliArgumentCombinationTests
{
    public static IEnumerable<object[]> ValidArgs()
    {
        yield return new object[] { new[] { "192.168.1.1", "-rc", "0:5" } };
        yield return new object[] { new[] { "192.168.1.1", "--read-discrete", "5", "--count", "2" } };
        yield return new object[] { new[] { "192.168.1.1", "-rh", "0:10", "--watch" } };
        yield return new object[] { new[] { "192.168.1.1", "-ri", "0:10", "--monitor", "0.5" } };
        yield return new object[] { new[] { "192.168.1.1", "--ref", "40001:10" } };
        yield return new object[] { new[] { "192.168.1.1", "--ref", "30001", "--count", "4" } };
        yield return new object[] { new[] { "192.168.1.1", "--analyze" } };
        yield return new object[] { new[] { "192.168.1.1", "--scan" } };
        yield return new object[] { new[] { "192.168.1.1", "--scan", "0.5" } };

        yield return new object[] { new[] { "192.168.1.1", "-wc", "12", "--on" } };
        yield return new object[] { new[] { "192.168.1.1", "--write-coil", "12", "--off" } };
        yield return new object[] { new[] { "192.168.1.1", "--write-coil", "12", "--value", "1" } };
        yield return new object[] { new[] { "192.168.1.1", "--write-coil", "12", "--raw16", "0xFF00" } };
        yield return new object[] { new[] { "192.168.1.1", "-wr", "40", "-d", "0x1234" } };
        yield return new object[] { new[] { "192.168.1.1", "-wmc", "60", "--bits", "1,0,1,1" } };
        yield return new object[] { new[] { "192.168.1.1", "--write-multi-coil", "60", "--bytes", "0x2D,0x01" } };
        yield return new object[] { new[] { "192.168.1.1", "-wmr", "80", "--data", "1,2,0xFF" } };

        yield return new object[] { new[] { "192.168.1.1", "--rtu", "--read-coil", "0:8" } };
        yield return new object[] { new[] { "192.168.1.1", "--tcp", "--read-holding", "1:2", "--u16", "--be" } };
        yield return new object[] { new[] { "/dev/ttyUSB0", "--read-inputreg", "0:8", "--baud", "19200", "--parity", "N", "--databits", "8", "--stopbits", "1" } };
        yield return new object[] { new[] { "--serial", "COM5", "--read-coil", "1", "--count", "5" } };

        yield return new object[] { new[] { "192.168.1.1", "--read-holding", "0:16", "--string", "--string-len", "12", "--null-term", "--encoding", "iso-8859-1" } };
        yield return new object[] { new[] { "192.168.1.1", "--read-coil", "0:16", "--bits" } };
        yield return new object[] { new[] { "192.168.1.1", "--read-coil", "0:16", "--hex" } };
        yield return new object[] { new[] { "192.168.1.1", "--read-holding", "0:8", "--bytes" } };
        yield return new object[] { new[] { "-vvv", "192.168.1.1", "--read-holding", "0:4", "--json", "--timestamp", "--quiet" } };
    }

    [Theory]
    [MemberData(nameof(ValidArgs))]
    public void Parse_ValidArgumentCombinations_NoParseErrors(string[] args)
    {
        var parseResult = Parse(args);
        Assert.Empty(parseResult.Errors);
    }

    [Fact]
    public void Invoke_FailsFast_WhenNoOperationProvided()
    {
        var root = Program.BuildRootCommand();
        var exitCode = root.Invoke(ArgumentNormalizer.Normalize(new[] { "192.168.1.1" }));
        Assert.Equal(1, exitCode);
    }

    [Fact]
    public void Invoke_FailsFast_WhenMultipleOperationsProvided()
    {
        var root = Program.BuildRootCommand();
        var exitCode = root.Invoke(ArgumentNormalizer.Normalize(new[] { "192.168.1.1", "--read-coil", "0", "--read-holding", "0" }));
        Assert.Equal(1, exitCode);
    }

    [Fact]
    public void Invoke_FailsFast_WhenScanAndReadAreCombined()
    {
        var root = Program.BuildRootCommand();
        var exitCode = root.Invoke(ArgumentNormalizer.Normalize(new[] { "192.168.1.1", "--scan", "--read-coil", "0:8" }));
        Assert.Equal(1, exitCode);
    }

    [Fact]
    public void Invoke_FailsFast_WhenSerialWithoutBaud()
    {
        var root = Program.BuildRootCommand();
        var exitCode = root.Invoke(ArgumentNormalizer.Normalize(new[] { "--serial", "/dev/ttyUSB0", "--read-coil", "0:8" }));
        Assert.Equal(1, exitCode);
    }

    [Fact]
    public void MilestoneTracker_CoversAllLongFlagsInHelpSpec()
    {
        var rootPath = FindRepositoryRoot();
        var helpPath = Path.Combine(rootPath, "HELP.md");
        var trackerPath = Path.Combine(rootPath, "docs", "FLAG_MILESTONES.md");

        var helpText = File.ReadAllText(helpPath);
        var trackerText = File.ReadAllText(trackerPath);

        var helpFlags = Regex.Matches(helpText, @"--[a-z][a-z0-9-]+")
            .Select(m => m.Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray();

        var trackerFlags = Regex.Matches(trackerText, @"\|\s+`(--[a-z][a-z0-9-]+)`\s+\|")
            .Select(m => m.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .ToHashSet(StringComparer.Ordinal);

        var missing = helpFlags.Where(flag => !trackerFlags.Contains(flag)).ToArray();
        Assert.True(missing.Length == 0, $"Missing flags in docs/FLAG_MILESTONES.md: {string.Join(", ", missing)}");

        var invalidStatuses = Regex.Matches(trackerText, @"\|\s+`--[a-z][a-z0-9-]+`\s+\|\s+([^|]+?)\s+\|")
            .Select(m => m.Groups[1].Value.Trim())
            .Where(status => status is not ("Implemented" or "Partial" or "Planned"))
            .Distinct()
            .ToArray();

        Assert.True(invalidStatuses.Length == 0, $"Invalid status values in docs/FLAG_MILESTONES.md: {string.Join(", ", invalidStatuses)}");
    }

    private static ParseResult Parse(string[] args)
    {
        var root = Program.BuildRootCommand();
        return root.Parse(ArgumentNormalizer.Normalize(args));
    }

    private static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "HELP.md")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root containing HELP.md");
    }
}
