mbdc - Modbus debug client (TCP / RTU / ASCII) with decoding and monitoring

USAGE:
  mbdc [TARGET] [TRANSPORT OPTIONS] [OPERATION] [DECODE OPTIONS] [OUTPUT OPTIONS]

TARGET:
  TARGET may be one of:
    host                 Modbus/TCP (default port 502)
    host:port            Modbus/TCP on custom port
    /dev/ttyXXX          Serial device (Linux)
  If TARGET is omitted, --serial must be provided (e.g. on Windows).

OPERATION (exactly one required):
  Reads:
    -rc, --read-coil SPEC             Read coils (FC01)
    -rd, --read-discrete SPEC         Read discrete inputs (FC02)
    -rh, --read-holding SPEC          Read holding registers (FC03)
    -ri, --read-inputreg SPEC         Read input registers (FC04)
        --ref SPEC                    Read using Modicon reference addressing (0xxxx/1xxxx/3xxxx/4xxxx)

  Writes:
    -wc, --write-coil ADDR            Write single coil (FC05)
    -wr, --write-reg  ADDR            Write single holding register (FC06)
    -wmc,--write-multi-coil ADDR      Write multiple coils (FC15)
    -wmr,--write-multi-reg  ADDR      Write multiple holding registers (FC16)

SPEC / ADDR / COUNT:
  Addressing is 0-based by default (protocol addressing).
  SPEC may be:
    ADDR                Start address
    ADDR:COUNT          Start address + count (overrides --count)

  Examples:
    -rh 0               Read holding register at addr 0 (count defaults to 1)
    -rh 0:10            Read 10 holding registers starting at addr 0
    --ref 40001         Read holding register 40001 (=> FC03 addr 0)
    --ref 40001:10      Read 10 registers starting at 40001 (=> addr 0 count 10)

  -c, --count N         Number of items to read (default: 1)
                        Ignored if SPEC includes ":COUNT".

UNIT / SLAVE ID:
  -u, --unit ID         Unit identifier / slave address (default: 1)
                        For Modbus/TCP this is the Unit ID field. For serial this is the slave address.

TRANSPORT OPTIONS:
  Modbus/TCP:
    (default when TARGET looks like host or host:port)
    --tcp               Force Modbus/TCP (even if TARGET ambiguous)
    -p, --port PORT     TCP port (default: 502)
    --timeout SEC       Response timeout (default: 1.0)
    --retries N         Retries on timeout (default: 0)

  RTU-over-TCP:
    --rtu               Use Modbus RTU framing over TCP stream (requires host/host:port)
                        Useful for serial device servers / gateways.

  Serial (RTU / ASCII):
    --serial DEV        Serial device (e.g. COM5 on Windows; /dev/ttyUSB0 on Linux)
                        If TARGET is a /dev/tty... path, it is used as the serial device.
    --ascii             Use Modbus ASCII (default is RTU)
    --baud RATE         Baud rate (default: 19200)
    --parity {N,E,O}    Parity (default: N)
    --databits {7,8}    Data bits (default: 8)
    --stopbits {1,2}    Stop bits (default: 1)

DATA INPUT (writes):
  Unless otherwise noted, numeric inputs accept:
    decimal (e.g. 1234), hex (0x04D2), or binary for bits (0b1010).

  Single coil write (FC05):
    -wc ADDR --on                    Write coil ON (FF00 on wire)
    -wc ADDR --off                   Write coil OFF (0000 on wire)
    -wc ADDR --value {0|1|true|false}
                                    Write coil using boolean value
    -wc ADDR --raw16 0xFF00          Write raw 16-bit coil payload (advanced)

  Single register write (FC06):
    -wr ADDR -d VALUE                Write 16-bit register value
                                    VALUE is one 16-bit word (0..65535)

  Multiple coil write (FC15):
    -wmc ADDR --bits BITSTRING        BITSTRING like: 10110
    -wmc ADDR --bits CSV             CSV like: 1,0,1,1,0
    -wmc ADDR --bytes HEXBYTES       Packed bytes, e.g. 0x2D or 0x2D,0x01
                                    (LSB-first packing per Modbus)

  Multiple register write (FC16):
    -wmr ADDR -d WORDS               WORDS is a list of 16-bit words
                                    Examples:
                                      -d 1,2,3
                                      -d 0x1234,0x0001,0x00FF
                                    Tip: quote lists to avoid shell surprises:
                                      -d '0x1234,0x0001'

  Common write flags:
    -d, --data X                      Data payload (meaning depends on operation)
        --value X                     Alias of --data (reserved; may be used for booleans/typed values later)
        --on / --off                  Convenience for --write-coil
        --raw16 X                     Raw 16-bit value (coil/reg payload)
        --raw-bytes X                 Raw bytes for diagnostic output modes (see OUTPUT)

DECODE OPTIONS (reads):
  By default mbdc prints raw values plus common interpretations.

  Register interpretation:
    --u16                Interpret registers as unsigned 16-bit (default view)
    --s16                Interpret as signed 16-bit
    --u32                Interpret as unsigned 32-bit (uses 2 registers)
    --s32                Interpret as signed 32-bit (uses 2 registers)
    --f32                Interpret as IEEE-754 float32 (uses 2 registers)
    --f64                Interpret as IEEE-754 float64 (uses 4 registers)

  Word/byte order helpers (register-based):
    --be                 Treat register bytes as big-endian within each 16-bit word (default)
    --le, --little-endian
                         Swap bytes within each 16-bit word (AB -> BA)

    --ws, --word-swap    Swap 16-bit word order in multi-register types (ABCD -> CDAB)
                         Applies to u32/s32/f32 (2 regs) and u64/s64/f64 (4 regs)

    --bs, --byte-swap    Alias of --le (byte swap within words)

  String / bytes decoding (register-based):
    --bytes              Print raw bytes for the read range (registers -> 2*N bytes)
    --string             Decode registers as string bytes (2 bytes per register)
    --string-len N       Limit output to N bytes (default: derived from count)
    --null-term          Stop string at first NUL (0x00)
    --encoding ENC       Character encoding for --string
                         Examples: utf-8, latin1, windows-1252, iso-8859-1, iso-8859-15
                         Note: many devices use ISO-8859-1/15 for åäö.

  Bit interpretation (coil/discrete reads):
    --bool               Print each bit as true/false (default)
    --bits               Print as 0/1 bitstring
    --hex                Print packed bytes in hex (diagnostic)

WATCH / MONITOR (continuous reads):
  These apply to read operations only.

    --watch [SEC]        Re-read continuously and append output (scrolling)
    --monitor [SEC]      Re-read continuously and redraw screen each interval (like "watch")
                         If SEC is omitted, default interval is 1.0 seconds.

  Examples:
    mbdc 192.168.1.1 -rh 0:40 --watch 0.5
    mbdc 192.168.1.1 -ri 0:60 --monitor 1
    mbdc 192.168.1.1 --ref 40001:40 --monitor

ADVANCED ANALYSIS:
    --diff               When watching/monitoring, highlight values that changed since last read
    --only-changed       Only print values that changed since last read
    --group N            Group output (e.g. 2 regs per value for f32 scanning)
    --scan {u16,s16,f32,...}
                         Interpret the whole range as repeated values of the chosen type
                         (useful when hunting PV/SP-like signals)

OUTPUT OPTIONS:
    -v                   Verbose (show transport, function, unit, request/response summary)
    -vv                  More verbose (show MBAP/RTU fields, timing, retries)
    -vvv                 Debug (dump raw frames / hex)
    -q, --quiet          Minimal output (values only)
    --json               JSON output (machine-readable; includes raw + decoded forms)
    --no-color           Disable ANSI colors
    --timestamp          Prefix each line with timestamp (useful with --watch)

COMMON OPTIONS:
    -h, --help           Show this help
    --version            Show version

NOTES:
  - Protocol addresses are 0-based. Many manuals use Modicon reference numbers:
      00001..  Coils            => FC01 addr = ref - 00001
      10001..  Discrete inputs  => FC02 addr = ref - 10001
      30001..  Input registers  => FC04 addr = ref - 30001
      40001..  Holding regs     => FC03 addr = ref - 40001
    Use --ref to accept those reference numbers directly.

  - Coils/discrete inputs are packed LSB-first in returned bytes.
    If you read addr A with count N, you get bits A..A+N-1.

EXAMPLES:
  Read 5 coils starting at coil 0 (TCP):
    mbdc 192.168.1.1 -rc 0:5

  Read holding registers 0..9 and decode as float32 scan (byte swap + word swap):
    mbdc 192.168.1.1 -rh 0:10 --scan f32 --le --ws

  Read reference 40001 (holding reg #1) and display raw + u16:
    mbdc 192.168.1.1 --ref 40001

  Watch a large register block while hunting PV/SP (show only changes):
    mbdc 192.168.1.1 -rh 0:200 --watch 0.5 --only-changed

  Decode a status message stored as two 16-byte null-terminated chunks (ISO-8859-1):
    mbdc 192.168.1.1 -rh 100:16 --string --string-len 32 --null-term --encoding iso-8859-1
