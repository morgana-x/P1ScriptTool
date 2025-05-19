namespace P1Script
{
    public class Util
    {
        public static string ReadStringArgString(StreamReader sr)
        {
            string str = "";
            int c = (int)'\0';
            while (c != -1 && (char)c != '"')
            {
                c = sr.Read();
                if (c == '\\')
                {
                    var cc = sr.Read();
                    if (cc == -1) break;
                    str += (char)(cc);
                    continue;
                }

                if ((char)c == '"')
                    break;
                
                str += (char)c;
            }
            return str;
        }

        public static string ReadByteArgsString(StreamReader sr)
        {
            string str = "";
            int c = sr.Read();

            while (c != -1 && (char)c != ')')
            {
                c = sr.Read();

                if ((char)c == '"')
                {
                    str += ReadStringArgString(sr);
                    continue;
                }

                if ((char)c != ')')
                    str += (char)c;
            }

            return str;
        }

        public static int SkipWhitespace(StreamReader sr)
        {
            int numLines = 1;
            int c = sr.Peek();
            while ( c != -1 && ((char)c == ';' || (char)c == '\n' || (char)c == ')'))
            {
                c = sr.Read();
                if ((char)c == '/' && (char)sr.Peek() == '/')
                {
                    while (c != -1 && (char)c != '\n')
                        c = sr.Read();
                }
                
            }
            return numLines;
        }
    }
}