using System.Text.RegularExpressions;

namespace ModbusDirectConnect.CLI.Infrastructure;

public static class ArgumentNormalizer
{
    private static readonly HashSet<string> TopLevelCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "read",
        "write",
        "help"
    };

    private static readonly Regex VerboseTokenPattern = new("^-v+$", RegexOptions.Compiled);

    public static string[] Normalize(string[] args)
    {
        var normalized = new List<string>();

        if (args.Length > 0 && IsImplicitTargetToken(args[0]))
        {
            normalized.Add("--target");
            normalized.Add(args[0]);

            for (var i = 1; i < args.Length; i++)
            {
                normalized.Add(args[i]);
            }
        }
        else
        {
            normalized.AddRange(args);
        }

        var verbosityCount = 0;
        var withoutVerboseShorthand = new List<string>();

        foreach (var token in normalized)
        {
            if (VerboseTokenPattern.IsMatch(token))
            {
                verbosityCount += token.Length - 1;
                continue;
            }

            withoutVerboseShorthand.Add(token);
        }

        if (verbosityCount > 0)
        {
            withoutVerboseShorthand.Add("--verbosity-level");
            withoutVerboseShorthand.Add(Math.Min(verbosityCount, 3).ToString());
        }

        return withoutVerboseShorthand.ToArray();
    }

    private static bool IsImplicitTargetToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (token.StartsWith("-", StringComparison.Ordinal))
        {
            return false;
        }

        return !TopLevelCommands.Contains(token);
    }
}
