public partial class Program
{
    public static void Main(string[] args)
    {
        string file = args.Length > 0 ? args[0] : "";
        string outfile = args.Length > 1 ? args[1] : "";

        if (file == "")
        {
            Console.WriteLine("Drag and drop the file to extract/repack");
            file = Console.ReadLine().Replace("\"", "");
        }

        if (!File.Exists(file))
        {
            Console.WriteLine($"File {file} doesn't exist!");
            return;
        }

        P1Script.P1Script script = new P1Script.P1Script(file);

        if (script.Binary)
        {
            Console.WriteLine("Writing decompiled file...");
            script.ExportSource(outfile == "" ? file.ToLower().Replace(".bin", "") : outfile);
            Console.WriteLine("Wrote decompiled file!");
            return;
        }

        Console.WriteLine("Writing compiled file...");
        P1Script.P1Script.ExportCompiled(file, outfile);
        Console.WriteLine("Wrote compiled file!");
    }
}
