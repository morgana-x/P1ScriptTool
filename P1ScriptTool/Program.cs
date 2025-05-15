public partial class Program
{
    public static void Main(string[] args)
    {
        string file = args.Length > 0 ? args[0] : "";
        if (file == "")
        {
            Console.WriteLine("Drag and drop the file to extract");
            file = Console.ReadLine().Replace("\"", "");
        }

        FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
        P1Script.P1Script script = new P1Script.P1Script(fs);
        script.Export(file.ToLower().Replace(".bin", ".txt"));
    }
}
