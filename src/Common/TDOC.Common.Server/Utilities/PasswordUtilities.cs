using System.Text;

namespace TDOC.Common.Server.Utilities
{
    internal class PasswordUtilities
    {
        private const string str64 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+-";
        private const string str64Old = "0IOlo";
        private const string str64New = "#/=%&";

        private const ushort cryptKey1 = 23114;
        private const ushort cryptKey2 = 19985;

        /// <summary>
        /// Decrypts a database password
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public static string DecryptDbPassword(string encrypted) => Decrypt(encrypted, CryptKeys.CryptKeyDb);

        public static string DecryptSslPassword(string encrypted) => Decrypt(encrypted, CryptKeys.CryptKeySsl);

        private static string Decrypt(string encryptedString, ushort cryptKey)
        {
            if (string.IsNullOrEmpty(encryptedString))
                throw new ArgumentException("Unable to decrypt empty string", nameof(encryptedString));

            string encryptPart = encryptedString[0] != '#' ? encryptedString : encryptedString.Substring(1);
            return DoDecrypt(encryptPart, cryptKey);
        }

        private static string DoDecrypt(string encryptedString, ushort cryptKey)
        {
            string s = Str64ToHex(encryptedString);
            byte[] chr = new byte[1];
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < s.Length / 2; i++)
            {
                byte j = byte.Parse(s.Substring(2 * i, 2), style: System.Globalization.NumberStyles.HexNumber);
                byte temp = (byte)(j ^ cryptKey >> 8);
                chr[0] = temp;
                stringBuilder.Append(Encoding.ASCII.GetString(chr));
                cryptKey = (ushort)((j + cryptKey) * cryptKey1 + cryptKey2);
            }
            return stringBuilder.ToString();
        }

        private static string Str64ToHex(string input)
        {
            int i = input.Length;
            if (i < 2)
                return string.Empty;
            int j = 1;
            var stringBuilder = new StringBuilder();
            while (j + 1 < i)
            {
                stringBuilder.Append(IntToHex(Str64ToInt(input.Substring(j, 2)), 3));
                j += 2;
            }
            if (j == i - 1)
                stringBuilder.Append(IntToHex(Str64ToInt(input.Substring(j, 1)), 1));

            return stringBuilder.ToString();
        }

        private static uint Str64ToInt(string input)
        {
            uint result = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                int j = str64.IndexOf(c);

                if (j == -1 && str64New.IndexOf(c) != -1)
                {
                    c = str64Old[str64New.IndexOf(c)];
                    j = str64.IndexOf(c);
                }

                result = (uint)(64 * result + j);
            }

            return result;
        }

        private static string IntToHex(uint value, int numberDigits) => value.ToString($"X{numberDigits}");

        internal class VersionPrefix
        {
            public const char Version15 = '9';
        }

        internal class CryptKeys
        {
            public const ushort CryptKey1500 = 2306;
            public const ushort CryptKeyDb = 11166;
            public const ushort CryptKeySsl = 11168;
        }
    }
}