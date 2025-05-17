using P1ScriptTool;

namespace P1Script
{
    public class P1Section
    {
        public List<P1SubSection> SubSections = new List<P1SubSection>();
        public long Offset;
        public long Size;
        

        public List<ushort> SubsectionSizes = new List<ushort>();
       // public const long Length = 0x1430;

        public P1Section(BinaryReader br, long Offset)
        {
            this.Offset = Offset;
            this.Size = Size;
            ReadBinary(br);
        }
        public P1Section(StreamReader sr)
        {
            ReadText(sr);
        }
        private void ReadBinary(BinaryReader br)
        {
            br.BaseStream.Position = Offset; // Meta data

            br.BaseStream.Position = Offset + 0x3C;

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                SubsectionSizes.Add(br.ReadUInt16());
                var val = br.ReadUInt16();
                if (val != 32784) break;
            }

            for (int i = 0; i < SubsectionSizes.Count; i++)
            {
                long offset = Offset + 0x1F8 + (i > 0 ? SubsectionSizes[i - 1] : 0);
                if (br.BaseStream.Position + offset > br.BaseStream.Length)
                    break;
                br.BaseStream.Position = offset;
                SubSections.Add(new P1SubSection(br, SubsectionSizes[i]));
            }

            long nextSubSection = br.BaseStream.Position + 0x146;

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
