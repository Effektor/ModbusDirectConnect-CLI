using System.CommandLine;
using ModbusDirectConnect.CLI.Client;

namespace ModbusDirectConnect.CLI.Commands;

public static class ReadCommands
{
    public static Command CreateReadCommand(
        Option<string> hostOption,
        Option<int> portOption,
        Option<byte> slaveIdOption,
        Option<int> timeoutOption,
        Option<string> protocolOption)
    {
        var readCommand = new Command("read", "Read data from Modbus devices");

        // read coils
        var readCoilsCommand = new Command("coils", "Read coils (function code 1)");
        var coilAddressArg = new Argument<ushort>("address", "Starting address");
        var coilCountArg = new Argument<ushort>("count", "Number of coils to read");
        readCoilsCommand.AddArgument(coilAddressArg);
        readCoilsCommand.AddArgument(coilCountArg);
        readCoilsCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, count) =>
        {
            try
            {
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                var results = await client.ReadCoilsAsync(address, count);
                PrintResults("Coils", address, results);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading coils: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, coilAddressArg, coilCountArg);

        // read discrete inputs
        var readDiscreteInputsCommand = new Command("discrete-inputs", "Read discrete inputs (function code 2)");
        var discreteAddressArg = new Argument<ushort>("address", "Starting address");
        var discreteCountArg = new Argument<ushort>("count", "Number of discrete inputs to read");
        readDiscreteInputsCommand.AddArgument(discreteAddressArg);
        readDiscreteInputsCommand.AddArgument(discreteCountArg);
        readDiscreteInputsCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, count) =>
        {
            try
            {
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                var results = await client.ReadDiscreteInputsAsync(address, count);
                PrintResults("Discrete Inputs", address, results);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading discrete inputs: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, discreteAddressArg, discreteCountArg);

        // read holding registers
        var readHoldingRegistersCommand = new Command("holding-registers", "Read holding registers (function code 3)");
        var holdingAddressArg = new Argument<ushort>("address", "Starting address");
        var holdingCountArg = new Argument<ushort>("count", "Number of holding registers to read");
        readHoldingRegistersCommand.AddArgument(holdingAddressArg);
        readHoldingRegistersCommand.AddArgument(holdingCountArg);
        readHoldingRegistersCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, count) =>
        {
            try
            {
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                var results = await client.ReadHoldingRegistersAsync(address, count);
                PrintResults("Holding Registers", address, results);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading holding registers: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, holdingAddressArg, holdingCountArg);

        // read input registers
        var readInputRegistersCommand = new Command("input-registers", "Read input registers (function code 4)");
        var inputAddressArg = new Argument<ushort>("address", "Starting address");
        var inputCountArg = new Argument<ushort>("count", "Number of input registers to read");
        readInputRegistersCommand.AddArgument(inputAddressArg);
        readInputRegistersCommand.AddArgument(inputCountArg);
        readInputRegistersCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, count) =>
        {
            try
            {
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                var results = await client.ReadInputRegistersAsync(address, count);
                PrintResults("Input Registers", address, results);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading input registers: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, inputAddressArg, inputCountArg);

        readCommand.AddCommand(readCoilsCommand);
        readCommand.AddCommand(readDiscreteInputsCommand);
        readCommand.AddCommand(readHoldingRegistersCommand);
        readCommand.AddCommand(readInputRegistersCommand);

        return readCommand;
    }

    private static void PrintResults<T>(string dataType, ushort startAddress, T[] results)
    {
        Console.WriteLine($"\n{dataType} (starting at address {startAddress}):");
        Console.WriteLine(new string('-', 50));
        
        for (int i = 0; i < results.Length; i++)
        {
            var address = startAddress + i;
            Console.WriteLine($"  Address {address}: {results[i]}");
        }
        
        Console.WriteLine();
    }
}
