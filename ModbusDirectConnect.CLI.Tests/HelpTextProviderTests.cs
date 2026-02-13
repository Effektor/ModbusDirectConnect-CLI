using ModbusDirectConnect.CLI.Infrastructure;

namespace ModbusDirectConnect.CLI.Tests;

public class HelpTextProviderTests
{
    [Fact]
    public void GetHelpText_ReadsEmbeddedHelpTxt()
    {
        var rootPath = FindRepositoryRoot();
        var expected = File.ReadAllText(Path.Combine(rootPath, "help.txt"));
        var actual = HelpTextProvider.GetHelpText();

        Assert.Equal(Normalize(expected), Normalize(actual));
    }

    private static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "help.txt")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root containing help.txt");
    }

    private static string Normalize(string input)
    {
        return input.Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
