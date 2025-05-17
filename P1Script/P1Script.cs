namespace P1Script
{
    public class P1Script
    {   


        public List<P1Section> Sections = new List<P1Section>();

        Stream stream;
        public P1Script(Stream stream)
        {
            this.stream = stream;
            ReadHeader();
        }
        public P1Script(string filePath)
        {
            this.stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            ReadHeader();
        }
        private void ReadHeader()
        {
            var br = new BinaryReader(stream);

            br.BaseStream.Position = 0;

            List<long> SectionOffsets = new List<long>();
            while (true)
            {
                ushort offset = br.ReadUInt16();
                if (offset == 0) break;
                SectionOffsets.Add(offset * 0x800);
            }

            for (int i=0; i < SectionOffsets.Count; i++)
                this.Sections.Add(new P1Section(br, SectionOffsets[i]));

            br.Dispose();
            return;   
        }
        public void ExportSource(string folder)  
        {
            string? dir = folder; //Path.GetDirectoryName(filepath); // https://github.com/shadow-nero - Fix UNIX FileStream Path not found error
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            for (int i = 0; i < Sections.Count; i++)
                  Sections[i].ExportText(Path.Join(folder, i.ToString()));
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


            writer.Dispose();
            writer.Close();
            reader.Dispose();
            reader.Close();
        }

        public void Dispose()
        {
            stream.Dispose();
            stream.Close();
        }
    }
}
