
## Read from a tcp slave

mbdc 192.168.1.1 --read-coil 1 --count 5

## Read from a rtu-over-tcp slave

mbdc 192.168.1.1 --rtu --read-coil 1 --count 5

## Read from a serial port on windows 

mbdc --serial com5 --read-coil 1 --count 5

## Read from a serial port on linux

mbdc /dev/serial1 --read-coil 1 --count 5

# Read from serial in ASCII-mode

mbdc /dev/serial1 --ascii --rc 2 -c 5

## Read-operations

--read-coil n, -rc n

--read-discrete n, -rd n

--read-holding n, -rh n

--read-inputreg n, -ri n

## Write-operations

--write-coil n --data 0x0014, -wc n -d 0x0014

--write-reg n --data 0x1234, -wr n -d 0x1234

--write-multi-coil n --data 1234

--write-multi-reg n --data 0xFFFF