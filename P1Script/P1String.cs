namespace P1Script
{
    public class P1String
    {
        public static string ReadBinaryString(BinaryReader br)
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

                if (currentByte == 0x80)
                {
                    int type = currentByte;
                    currentByte = br.ReadByte();
                    str += GetCharString(currentByte, type);
                    continue;
                }

                str += GetCharString(currentByte);
            }

            return str;
        }

        public static string GetCharString(int chr, int type=0)
        {
            if ( type == 0x80 && CharsetUpper.ContainsKey(chr))
                return (CharsetUpper[chr]).ToString();

            if (type == 0x80 && CharsetSpecial.ContainsKey(chr))
                return (CharsetSpecial[chr]);

            if (CharsetLower.ContainsKey(chr))
                return CharsetLower[chr].ToString();

            if (CharsetUpper.ContainsKey(chr))
                return CharsetUpper[chr].ToString();

            if (type != 0)
                return $"0x{type.ToString("X")}(0x{chr.ToString("X")})";

            return $"0x{chr.ToString("X")}";
        }

        public static Dictionary<int, char> CharsetLower = new Dictionary<int, char>()
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

        };
        public static Dictionary<int, char> CharsetUpper = new Dictionary<int, char>()
        {
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

            [0xD0] = '?',
            [0xD1] = '!',

            [0xEA] = '□',
            [0xEB] = '×',
            [0xEC] = '○',
            [0xEE] = '△',
        };

        public static Dictionary<int, string> CharsetSpecial = new Dictionary<int, string>()
        {
            [0x2] = "[END]",
            [0xC1] = "[VAR1]",
            [0xC2] = "[VAR2]",
        };


    }
}
