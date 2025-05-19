using P1ScriptTool;
using System.IO;

namespace P1Script
{
    public class P1Section
    {
 
        public long Pointer;
        public long EndPointer;
        public long Size;

        public byte[] Header = new byte[0x1F8];
        public byte[] Metadata = new byte[0xED0];
        public List<P1Opcode> Opcodes = new List<P1Opcode>();

        public P1Section(BinaryReader br, long Offset, long EndOffset)
        {
            this.Pointer = Offset;
            this.EndPointer = EndOffset;
            Size = (EndPointer - Pointer);
            ReadBinary(br);
        }
        public P1Section(StreamReader sr)
        {
            ReadText(sr);
        }
        private void ReadBinary(BinaryReader br)
        {
            br.BaseStream.Position = Pointer; // Meta data
            Console.WriteLine("Pointer " + Pointer.ToString("X"));
            if (br.BaseStream.Position >= br.BaseStream.Length)
            {
                Console.WriteLine("Uh oh at end of file!");
                return;
            }

            Header = br.ReadBytes(0x1F8);
            Metadata = br.ReadBytes(0xED0);

            while (br.BaseStream.Position < br.BaseStream.Length && br.BaseStream.Position < EndPointer)
            {
                var opcode = P1Opcode.ReadOpcode(br);
                if (opcode == null) break;
                Opcodes.Add(opcode);
            }

            br.BaseStream.Position = Pointer + 4;
            //ushort Size = br.ReadUInt16();

            br.BaseStream.Position = Pointer + 10;
            //DataStart = Offset + (br.ReadUInt16() * 0xF8);

            br.BaseStream.Position = Pointer + 18;
            // DataEnd = Offset + (br.ReadUInt16() * 0xF8);

            br.BaseStream.Position = Pointer + 28;

         
        }

        private void ReadText(StreamReader sr)
        {
            int numLines = 0;
            while (!sr.EndOfStream && sr.Peek() != -1)
            {
                var opcode = P1Opcode.ReadOpcode(sr, ref numLines);
                if (opcode == null) break;
                Opcodes.Add(opcode);
            }
        }

        public void ExportText(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (var textWriter = new StreamWriter(Path.Join(folder, "data.txt")))
                foreach (var opcode in Opcodes)
                    textWriter.WriteLine(opcode.ToString());


            File.WriteAllBytes(Path.Join(folder, "header.bin"), Header);
            File.WriteAllBytes(Path.Join(folder, "meta.bin"), Metadata);
        }
        public void Import(string folder)
        {
            Header = File.ReadAllBytes(Path.Join(folder, "header.bin"));
            Metadata = File.ReadAllBytes(Path.Join(folder, "meta.bin"));

            using (var textReader = new StreamReader(Path.Join(folder, "data.txt")))
                ReadText(textReader);

        }
        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(Header);
            bw.Write(Metadata);
            foreach (var op in Opcodes)
                op.WriteBinary(bw);
        }


    }
}
