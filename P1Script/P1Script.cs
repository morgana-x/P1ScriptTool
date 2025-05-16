using P1ScriptTool;

namespace P1Script
{
    public class P1Script
    {
        public List<P1Opcode> opcodes = new List<P1Opcode>();

        public bool Binary = false;

        Stream stream;
        public P1Script(Stream stream, bool binary=true)
        {
            this.stream = stream;
            ReadHeader(binary);
        }
        public P1Script(string filePath)
        {
            this.stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            ReadHeader(filePath.ToLower().EndsWith(".bin"));
        }
        private void ReadHeader(bool binary)
        {
            Binary = binary;
            if (!binary) return;

            var br = new BinaryReader(stream);
            ReadBinary(br);
            br.Dispose();
            
            return;   
        }
        public void ExportSource(string filepath)
        {
            StreamWriter writer = new StreamWriter(filepath, false);
            Export(writer);
            writer.Dispose();
            writer.Close();
        }
        public static void ExportCompiled(string readPath, string outPath="")
        {
            if (outPath == "")
                outPath = readPath.Replace(".txt", ".bin");
            
            if (!Directory.Exists(Directory.GetParent(outPath).FullName))
                Directory.CreateDirectory(Directory.GetParent(outPath).FullName);

            FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(fs);

            var reader = new StreamReader(readPath);

            Export(reader, writer);

            writer.Dispose();
            writer.Close();
            reader.Dispose();
            reader.Close();
        }
        public void Export(StreamWriter writer)
        {
            foreach(var opcode in opcodes) 
                writer.WriteLine(opcode.ToString());
        }
        
        public static void Export(StreamReader sr, BinaryWriter writer)
        {
            int numLines = 0;
            while (!sr.EndOfStream && sr.Peek() != -1)
            {
                var opcode = P1Opcode.ReadOpcode(sr, ref numLines);
                if (opcode == null) break;

                opcode.WriteBinary(writer);
            }
        }
        private void ReadBinary(BinaryReader br)
        {
            opcodes.Clear();
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            while (br.BaseStream.Position < br.BaseStream.Length)
                opcodes.Add(P1Opcode.ReadOpcode(br));
        }

        public void Dispose()
        {
            stream.Dispose();
            stream.Close();
        }
    }
}
