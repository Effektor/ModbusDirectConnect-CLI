using System.Reflection;

namespace ModbusDirectConnect.CLI.Infrastructure;

public static class HelpTextProvider
{
    private const string ResourceName = "ModbusDirectConnect.CLI.help.txt";

    public static string GetHelpText()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(ResourceName);
        if (stream is null)
        {
            return "Help text resource missing. See help.txt in the repository.";
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
