using System.Reflection;

namespace ModbusDirectConnect.CLI.Infrastructure;

public static class HelpTextProvider
{
    private const string ResourceName = "ModbusDirectConnect.CLI.HELP.md";

    public static string GetHelpText()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(ResourceName);
        if (stream is null)
        {
            return "Help text resource missing. See HELP.md in the repository.";
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
