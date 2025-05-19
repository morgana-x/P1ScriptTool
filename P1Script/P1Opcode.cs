using P1Script;

namespace P1ScriptTool
{
    public class P1Opcode // Opcodes are defined as coming after a FF/255 byte
    {
        public static Dictionary<int, P1Opcode> Opcodes = new Dictionary<int, P1Opcode>()
        {   // More opcodes can be defined here
            [0x01] = new P1Opcode("close"),
            [0x02] = new P1Opcode("key"),
            [0x03] = new P1Opcode("n"),
            [0x04] = new P1Opcode("clear"),
            [0x05] = new P1Opcode("wait", 2),
            [0x06] = new P1Opcode("color", 2),

            [0x07] = new P1Opcode("firstname"),
            [0x08] = new P1Opcode("nickname"),

            [0x0E] = new P1Opcode("choice"),
            [0x0F] = new P1Opcode("lastname"),

            // [0x22] = new P1Opcode("loadtextend?", 6),
            [0x55] = new P1Opcode("text_load", 6), // Qlonever - FF 55 is a text box command that uses 6 bytes of data, the last four bytes of which are an offset within PSX memory to read text from (the current scene file is always loaded at offset 0x8010000)
                                                  // In over words last 4 bytes is a 32 bit integer
            [0x67] = new P1Opcode("text_size", 2)
        };

        public string Name;
        public int    NumArgs;
        public List<byte> Args = new List<byte>();
        public int Opcode { get {
                var result = Opcodes.Where(x => x.Value.Name == this.Name);
                if (result.Count() > 0) return result.First().Key;

                return byte.Parse(Name.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber); 
            } 
        }

        public string Text = string.Empty; // Reserved for "text" opcode
        public P1Opcode(string name, int numArgs=0)
        {
            Name = name; NumArgs = numArgs;
        }
        public P1Opcode(string name, string text)
        {
            Name = name; Text = text; NumArgs = -1;
        }
        public override string ToString()
        {
            return $"{Name}({ ((Text == string.Empty) ? string.Join(", ", Args) : $"\"{Text}\"")});";
        }

        public void WriteBinary(BinaryWriter bw)
        {
            if (this.Text != string.Empty)
            {
                bw.Write(P1String.ToPersonaString(Text));
                return;
            }
            bw.Write((byte)0xFF);
            bw.Write((byte)Opcode);
            foreach (var arg in Args)
                bw.Write(arg);
        }

        public static P1Opcode ReadOpcode(BinaryReader br) // Read from binary
        {
            int nextByte = br.ReadByte();

            if (nextByte != 0xFF)
            {
                br.BaseStream.Position -= 1;
                return (new P1Opcode("text", P1String.ReadPersonaString(br)));
            }

            byte b = br.ReadByte();

            P1Opcode opcodebase = Opcodes.ContainsKey(b) ? Opcodes[b] : new P1Opcode( "0x" + b.ToString("X"), -1);
            List<byte> args = new List<byte>();

            if (opcodebase.NumArgs == -1)
            {
                byte argb = 0;
                while (argb != 0xFF && br.BaseStream.Position < br.BaseStream.Length)
                {
                    argb = br.ReadByte();
                    if (argb == 0xFF || argb == -1) { br.BaseStream.Position -= 1; break; };
                    args.Add(argb);
                }
            }

            for (int i = 0;  i < opcodebase.NumArgs; i++)
                args.Add(br.ReadByte());

            return new P1Opcode(opcodebase.Name, opcodebase.NumArgs) { Args = args };
        }
     
        public static P1Opcode? ReadOpcode(StreamReader sr, ref int numLines) // read from text
        {
            numLines += Util.SkipWhitespace(sr);

            if (sr.EndOfStream)
                return null;

            string opcodename = "";
            List<byte> arguments = new List<byte>();

            while (sr.Peek() != '(' && sr.Peek()!= -1)
                opcodename += (char)sr.Read();

            opcodename = opcodename.Trim();

            if (opcodename == "text")
                return new P1Opcode("text", Util.ReadByteArgsString(sr));


            if (!opcodename.ToLower().StartsWith("0x") && Opcodes.Where((x) => x.Value.Name == opcodename).Count() == 0)
            {
                Console.WriteLine($"Script Compile Error at line {numLines}!, {opcodename} is not a valid opcode!");
                return null;
            }
    
            foreach (var arg in Util.ReadByteArgsString(sr).Replace(" ", "").Trim().Split(","))
            {
                if (arg == "") continue;
                try
                {
                    arguments.Add(byte.Parse(arg));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Script Compile Error at line {numLines}!, {arg} is not a valid argument byte at opcode {opcodename}!");
                    return null;
                }
            }


            var oldOpcode = !opcodename.ToLower().StartsWith("0x") ? Opcodes.Where((x) => x.Value.Name == opcodename).First().Value : new P1Opcode(opcodename, -1) {  };
            if (oldOpcode.NumArgs != -1 && arguments.Count != oldOpcode.NumArgs)
            {
                throw (new Exception($"Script Compile Error!, Incorrect length of args!") { });
            }
            return new P1Opcode(oldOpcode.Name, oldOpcode.NumArgs) { Args = arguments };

        }
    }
}