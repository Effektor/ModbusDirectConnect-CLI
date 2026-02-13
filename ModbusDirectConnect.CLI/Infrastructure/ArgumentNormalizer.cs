using System.Text.RegularExpressions;

namespace ModbusDirectConnect.CLI.Infrastructure;

public static class ArgumentNormalizer
{
    private static readonly HashSet<string> TopLevelCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "help"
    };

    private static readonly Regex VerboseTokenPattern = new("^-v+$", RegexOptions.Compiled);

    public static string[] Normalize(string[] args)
    {
        var verbosityCount = 0;
        var withoutVerboseShorthand = new List<string>();

        foreach (var token in args)
        {
            if (VerboseTokenPattern.IsMatch(token))
            {
                verbosityCount += token.Length - 1;
                continue;
            }

            withoutVerboseShorthand.Add(token);
        }

        var normalized = new List<string>();

        if (withoutVerboseShorthand.Count > 0 && IsImplicitTargetToken(withoutVerboseShorthand[0]))
        {
            normalized.Add("--target");
            normalized.Add(withoutVerboseShorthand[0]);
            for (var i = 1; i < withoutVerboseShorthand.Count; i++)
            {
                normalized.Add(withoutVerboseShorthand[i]);
            }
        }
        else
        {
            normalized.AddRange(withoutVerboseShorthand);
        }

        if (verbosityCount > 0)
        {
            normalized.Add("--verbosity-level");
            normalized.Add(Math.Min(verbosityCount, 3).ToString());
        }

        return normalized.ToArray();
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
