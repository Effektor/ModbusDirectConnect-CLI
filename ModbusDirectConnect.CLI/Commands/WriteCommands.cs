using System.CommandLine;
using ModbusDirectConnect.CLI.Client;

namespace ModbusDirectConnect.CLI.Commands;

public static class WriteCommands
{
    public static Command CreateWriteCommand(
        Option<string> hostOption,
        Option<int> portOption,
        Option<byte> slaveIdOption,
        Option<int> timeoutOption,
        Option<string> protocolOption)
    {
        var writeCommand = new Command("write", "Write data to Modbus devices");

        // write single coil
        var writeSingleCoilCommand = new Command("coil", "Write single coil (function code 5)");
        var coilAddressArg = new Argument<ushort>("address", "Address to write");
        var coilValueArg = new Argument<bool>("value", "Value to write (true/false or 1/0)");
        writeSingleCoilCommand.AddArgument(coilAddressArg);
        writeSingleCoilCommand.AddArgument(coilValueArg);
        writeSingleCoilCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, value) =>
        {
            try
            {
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                await client.WriteSingleCoilAsync(address, value);
                Console.WriteLine($"Successfully wrote coil at address {address}: {value}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing coil: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, coilAddressArg, coilValueArg);

        // write single register
        var writeSingleRegisterCommand = new Command("register", "Write single holding register (function code 6)");
        var registerAddressArg = new Argument<ushort>("address", "Address to write");
        var registerValueArg = new Argument<ushort>("value", "Value to write");
        writeSingleRegisterCommand.AddArgument(registerAddressArg);
        writeSingleRegisterCommand.AddArgument(registerValueArg);
        writeSingleRegisterCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, value) =>
        {
            try
            {
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                await client.WriteSingleRegisterAsync(address, value);
                Console.WriteLine($"Successfully wrote register at address {address}: {value}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing register: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, registerAddressArg, registerValueArg);

        // write multiple coils
        var writeMultipleCoilsCommand = new Command("coils", "Write multiple coils (function code 15)");
        var coilsAddressArg = new Argument<ushort>("address", "Starting address");
        var coilsValuesArg = new Argument<string>("values", "Comma-separated boolean values (e.g., true,false,true or 1,0,1)");
        writeMultipleCoilsCommand.AddArgument(coilsAddressArg);
        writeMultipleCoilsCommand.AddArgument(coilsValuesArg);
        writeMultipleCoilsCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, valuesStr) =>
        {
            try
            {
                var values = ParseBooleanArray(valuesStr);
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                await client.WriteMultipleCoilsAsync(address, values);
                Console.WriteLine($"Successfully wrote {values.Length} coils starting at address {address}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing multiple coils: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, coilsAddressArg, coilsValuesArg);

        // write multiple registers
        var writeMultipleRegistersCommand = new Command("registers", "Write multiple holding registers (function code 16)");
        var registersAddressArg = new Argument<ushort>("address", "Starting address");
        var registersValuesArg = new Argument<string>("values", "Comma-separated register values (e.g., 100,200,300)");
        writeMultipleRegistersCommand.AddArgument(registersAddressArg);
        writeMultipleRegistersCommand.AddArgument(registersValuesArg);
        writeMultipleRegistersCommand.SetHandler(async (host, port, slaveId, timeout, protocol, address, valuesStr) =>
        {
            try
            {
                var values = ParseUshortArray(valuesStr);
                var client = ModbusClientFactory.CreateClient(host, port, slaveId, timeout, protocol);
                await client.WriteMultipleRegistersAsync(address, values);
                Console.WriteLine($"Successfully wrote {values.Length} registers starting at address {address}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing multiple registers: {ex.Message}");
                Environment.Exit(1);
            }
        }, hostOption, portOption, slaveIdOption, timeoutOption, protocolOption, registersAddressArg, registersValuesArg);

        writeCommand.AddCommand(writeSingleCoilCommand);
        writeCommand.AddCommand(writeSingleRegisterCommand);
        writeCommand.AddCommand(writeMultipleCoilsCommand);
        writeCommand.AddCommand(writeMultipleRegistersCommand);

        return writeCommand;
    }

    private static bool[] ParseBooleanArray(string valuesStr)
    {
        var parts = valuesStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new bool[parts.Length];
        
        for (int i = 0; i < parts.Length; i++)
        {
            if (bool.TryParse(parts[i], out var boolValue))
            {
                result[i] = boolValue;
            }
            else if (int.TryParse(parts[i], out var intValue))
            {
                result[i] = intValue != 0;
            }
            else
            {
                throw new ArgumentException($"Invalid boolean value: {parts[i]}");
            }
        }
        
        return result;
    }

    private static ushort[] ParseUshortArray(string valuesStr)
    {
        var parts = valuesStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new ushort[parts.Length];
        
        for (int i = 0; i < parts.Length; i++)
        {
            if (!ushort.TryParse(parts[i], out result[i]))
            {
                throw new ArgumentException($"Invalid register value: {parts[i]}");
            }
        }
        
        return result;
    }
}
