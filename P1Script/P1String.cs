namespace P1Script
{
    public class P1String
    {
        public static string ReadPersonaString(BinaryReader br)
        {
            string str = "";
            int currentByte = 0;
            while (currentByte != 0xFF && br.BaseStream.Position < br.BaseStream.Length)
            {
                currentByte = (int)br.ReadByte();

                if (currentByte == 0xFF)
                {
                    br.BaseStream.Position -= 1;
                    break;
                }

                str += FromPersonaChar(currentByte);
            }

            return str;
        }
    
        public static byte[] ToPersonaString(string str)
        {
            List<byte> byteList = new List<byte>();
            for (int i=0; i<str.Length; i++)
            {
                if ( i+3 < str.Length && str[i] == '0' && str[i+1] == 'x' )
                {
                    byteList.Add(byte.Parse( str[i+2].ToString() + str[i+3].ToString(),System.Globalization.NumberStyles.HexNumber));
                    i += 3;
                    continue;
                }

                var c = ToPersonaChar(str[i], CharSet);

                if (c == -1)
                    continue;

                byteList.Add((byte)c); 
            }
            return byteList.ToArray();
        }

        private static int ToPersonaChar(char c, Dictionary<int, char> dict)
        {
            foreach (var a in dict)
            {
                if (a.Value == c)
                    return a.Key;
            }
            return -1;
        }

        public static string FromPersonaChar(int chr)
        {
            if (CharSet.ContainsKey(chr))
            {
                string str = CharSet[chr].ToString();
                if (str == "\"") str = "\\\"";
                return str;
            }

            return $"0x{chr.ToString("X2")}";
        }

        public static Dictionary<int, char> CharSet = new Dictionary<int, char>()
        {
            [0] = ' ',

            [0x31] = 'a',
            [0x32] = 'b',
            [0x33] = 'c',
            [0x34] = 'd',
            [0x35] = 'e',
            [0x36] = 'f',
            [0x37] = 'g',
            [0x38] = 'h',
            [0x39] = 'i',
            [0x3A] = 'j',
            [0x3B] = 'k',
            [0x3C] = 'l',
            [0x3D] = 'm',
            [0x3E] = 'n',
            [0x3F] = 'o',
            [0x40] = 'p',
            [0x41] = 'q',
            [0x42] = 'r',
            [0x43] = 's',
            [0x44] = 't',
            [0x45] = 'u',
            [0x46] = 'v',
            [0x47] = 'w',
            [0x48] = 'x',
            [0x49] = 'y',
            [0x4A] = 'z',

            [0x61] = '\'',
            [0x65] = ',',
            [0x68] = ':',

            [0x80] = '', // There is an invisible padding character here, comes before alot of capitals in vanilla scripts for some reason

            [0xA5] = '.',

            [0xA6] = 'A',
            [0xA7] = 'B',
            [0xA8] = 'C',
            [0xA9] = 'D',
            [0xAA] = 'E',
            [0xAB] = 'F',
            [0xAC] = 'G',
            [0xAD] = 'H',
            [0xAE] = 'I',
            [0xAF] = 'J',
            [0xB0] = 'K',
            [0xB1] = 'L',
            [0xB2] = 'M',
            [0xB3] = 'N',
            [0xB4] = 'O',
            [0xB5] = 'P',
            [0xB6] = 'Q',
            [0xB7] = 'R',
            [0xB8] = 'S',
            [0xB9] = 'T',
            [0xBA] = 'U',
            [0xBB] = 'V',
            [0xBC] = 'W',
            [0xBD] = 'X',
            [0xBE] = 'Y',
            [0xBF] = 'Z',

            [0xC0] = '0',
            [0xC1] = '1',
            [0xC2] = '2',
            [0xC3] = '3',
            [0xC4] = '4',
            [0xC5] = '5',
            [0xC6] = '6',
            [0xC7] = '7',
            [0xC8] = '8',
            [0xC9] = '9',

            [0xD0] = '?',
            [0xD1] = '!',
            [0xD8] = '(',
            [0xD9] = ')',
            [0xDb] = '"',

            [0xE1] = '-',
            [0xE7] = '/',
            [0xEA] = '□', // might have the order of these wrong
            [0xEB] = '×',
            [0xEC] = '○',
            [0xEE] = '△',
        };
    }
}
