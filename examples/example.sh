#!/bin/bash
# Example script demonstrating various Modbus operations using the CLI

# Configuration
HOST="192.168.1.100"
PORT=502
SLAVE=1

echo "=== ModbusDirectConnect CLI Examples ==="
echo ""

# Example 1: Read coils
echo "1. Reading 10 coils starting at address 0:"
modbus-cli read coils 0 10 --host $HOST --port $PORT --slave $SLAVE
echo ""

# Example 2: Read holding registers
echo "2. Reading 5 holding registers starting at address 1000:"
modbus-cli read holding-registers 1000 5 --host $HOST --port $PORT --slave $SLAVE
echo ""

# Example 3: Write a single coil
echo "3. Writing a single coil at address 0 (value: true):"
modbus-cli write coil 0 true --host $HOST --port $PORT --slave $SLAVE
echo ""

# Example 4: Write a single register
echo "4. Writing a single register at address 1000 (value: 12345):"
modbus-cli write register 1000 12345 --host $HOST --port $PORT --slave $SLAVE
echo ""

# Example 5: Write multiple coils
echo "5. Writing multiple coils starting at address 0:"
modbus-cli write coils 0 "true,false,true,true,false" --host $HOST --port $PORT --slave $SLAVE
echo ""

# Example 6: Write multiple registers
echo "6. Writing multiple registers starting at address 1000:"
modbus-cli write registers 1000 "100,200,300,400,500" --host $HOST --port $PORT --slave $SLAVE
echo ""

# Example 7: Read discrete inputs
echo "7. Reading 8 discrete inputs starting at address 100:"
modbus-cli read discrete-inputs 100 8 --host $HOST --port $PORT --slave $SLAVE
echo ""

# Example 8: Read input registers
echo "8. Reading 4 input registers starting at address 2000:"
modbus-cli read input-registers 2000 4 --host $HOST --port $PORT --slave $SLAVE
echo ""

echo "=== Examples completed ==="
