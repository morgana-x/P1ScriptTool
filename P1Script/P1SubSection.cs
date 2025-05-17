using P1ScriptTool;

namespace P1Script
{
    public class P1SubSection
    {
        public byte[] Metadata;
        public List<P1Opcode> Opcodes = new List<P1Opcode>();
        public P1SubSection(BinaryReader br, long Offset, ushort size)
        {
            long SectionEndPointer = Offset + size;// opcodeSectionLength;

            br.BaseStream.Position = Offset;

            Metadata = br.ReadBytes(0xED0);

            //br.BaseStream.Position = Offset + 0x10c8;

            while (br.BaseStream.Position < br.BaseStream.Length && br.BaseStream.Position < SectionEndPointer)
            {
                var opcode = P1Opcode.ReadOpcode(br);
                if (opcode == null) break;
                Opcodes.Add(opcode);
            }
        }
        public long Size()
        {
            long totalSize = Metadata.Length;

            foreach (var op in Opcodes)
                totalSize += 4 + (op.NumArgs);

            return totalSize;
        }
        public void WriteBinary(BinaryWriter bw)
        {
            foreach (var op in Opcodes)
                op.WriteBinary(bw);
        }

        public void WriteOpcodeText(StreamWriter sw)
        {
            foreach (var opcode in Opcodes)
                sw.WriteLine(opcode.ToString());
        }

        public void ExportToFolder(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            StreamWriter textWriter = new StreamWriter(Path.Join(folder, "data.txt"));
            WriteOpcodeText(textWriter);
            textWriter.Dispose();
            textWriter.Close();

            File.WriteAllBytes(Path.Join(folder, "meta.bin"), Metadata);
        }


    }
}
