using P1ScriptTool;

namespace P1Script
{
    public class P1Script
    {
        public Stream stream;
        public BinaryReader br;

        public List<Opcode> opcodes = new List<Opcode>();
        public P1Script(Stream stream)
        {
            this.stream = stream;
            this.br = new BinaryReader(stream);
            ReadBinary();
        }
        
        public void Export(string filename)
        {
            StreamWriter writer = new StreamWriter(filename, false);
            Export(writer);
            writer.Dispose();
            writer.Close();
        }
        public void Export(StreamWriter writer)
        {
            foreach(var opcode in opcodes) 
                writer.WriteLine(opcode.ToString());
        }
        private void ReadBinary()
        {
            opcodes.Clear();
            this.br.BaseStream.Seek(0, SeekOrigin.Begin);
            //this.br.BaseStream.Position = 0x18D6;
            while (br.BaseStream.Position < br.BaseStream.Length)
                opcodes.Add(Opcode.ReadOpcode(br));
        }
    }
}
