using P1Script;

namespace P1ScriptTool
{
    public class Opcode // Opcodes are defined as coming after a FF/255 byte
    {
        public string Name;
        public int    ArgLength;
        public List<byte> Args = new List<byte>();

        public string Text = string.Empty; // Reserved for "text" opcode
        public Opcode(string name, int argLength=0)
        {
            Name = name; ArgLength = argLength;
        }
        public Opcode(string name, string text)
        {
            Name = name; Text = text; ArgLength = -1;
        }
        public override string ToString()
        {
            return $"{Name}({ ((Text == string.Empty) ? string.Join(", ", Args) : $"\"{Text}\"")});";
        }

        public static Dictionary<int, Opcode> Opcodes = new Dictionary<int, Opcode>()
        {
            [0x01] = new Opcode("close"),
            [0x02] = new Opcode("key"),
            [0x03] = new Opcode("n"),
            [0x04] = new Opcode("clear"),
            [0x05] = new Opcode("wait", 2),
            [0x06] = new Opcode("color", 2),
            [0x07] = new Opcode("name"),
            [0x0E] = new Opcode("choice")
        };

        public static Opcode ReadOpcode(BinaryReader br)
        {
            int nextByte = br.ReadByte();

            if (nextByte != 0xFF)
            {
                br.BaseStream.Position -= 1;
                return (new Opcode("text", P1String.ReadBinaryString(br)));
            }

            byte b = br.ReadByte();

            Opcode opcodebase = Opcodes.ContainsKey(b) ? Opcodes[b] : new Opcode( "0x" + b.ToString("X"), -1);
            List<byte> args = new List<byte>();

            if (opcodebase.ArgLength == -1)
            {
                byte argb = 0;
                while (argb != 0xFF)
                {
                    argb = br.ReadByte();
                    if (argb == 0xFF) { br.BaseStream.Position -= 1; break; };
                    args.Add(argb);
                }
            }

            for (int i = 0;  i < opcodebase.ArgLength; i++)
                args.Add(br.ReadByte());

            return new Opcode(opcodebase.Name, opcodebase.ArgLength) { Args = args };
        }

    }
}
