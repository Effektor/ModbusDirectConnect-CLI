namespace ModbusDirectConnect.CLI.Infrastructure;

public sealed class VerbosityLogger(int level)
{
    public int Level { get; } = Math.Clamp(level, 0, 3);

    public void Log(int minimumLevel, string message)
    {
        if (Level < minimumLevel)
        {
            return;
        }

        Console.Error.WriteLine(message);
    }
}
