using P1ScriptTool;

namespace P1Script
{
    public class P1Section
    {
        public List<P1SubSection> SubSections = new List<P1SubSection>();
        public long Offset;
        public ushort Size;
        

        public List<ushort> SubsectionSizes = new List<ushort>();
       // public const long Length = 0x1430;

        public P1Section(BinaryReader br, long Offset)
        {
            this.Offset = Offset;
            ReadBinary(br);
        }
        public P1Section(StreamReader sr)
        {
            ReadText(sr);
        }
        private void ReadBinary(BinaryReader br)
        {
            br.BaseStream.Position = Offset; // Meta data

            if (br.BaseStream.Position >= br.BaseStream.Length) return;

            br.BaseStream.Position = Offset + 4;
            ushort Size = br.ReadUInt16();



            SubSections.Add(new P1SubSection(br, Offset + 0x1F8, (ushort)(Size-0x1F8)));
        }

        private void ReadText(StreamReader sr)
        {
            int numLines = 0;
            while (!sr.EndOfStream && sr.Peek() != -1)
            {
                var opcode = P1Opcode.ReadOpcode(sr, ref numLines);
                if (opcode == null) break;
         //       Opcodes.Add(opcode);
            }
        }
        public void WriteBinary(BinaryWriter bw)
        {
            foreach (var section in SubSections)
                section.WriteBinary(bw);
        }
        public void ExportText(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            for (int i=0; i < SubSections.Count;i++)
                SubSections[i].ExportToFolder(Path.Join(folder, i.ToString()));
        }


    }
}
