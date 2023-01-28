using System.Security.Cryptography;
using System.Text;

namespace AdventGamesCore.Extensions
{
    public static class StringExtensions
    {
        public const string NumericFormat = "#,0.##";
        private static readonly string encryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static bool IsNullOrBlank(this string text)
        {
            return string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
        }

        public static string Encrypt(this string plainText)
        {
            if (plainText.IsNullOrBlank())
            {
                return plainText;
            }

            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(
                    password: encryptionKey,
                    salt: new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new())
                {
                    using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plainText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return plainText;
        }

        public static string Decrypt(this string encodedData)
        {
            if (encodedData.IsNullOrBlank())
            {
                return encodedData;
            }

            encodedData = encodedData.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(encodedData);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(
                    password: encryptionKey,
                    salt: new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new())
                {
                    using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encodedData = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encodedData;
        }

        public static string BitShift(this string text)
        {
            int shft = 5;
            string encrypted = text.Select(ch => ch << shft).Aggregate("", (current, val) => current + (char)(val * 2));
            encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(encrypted));

            return encrypted;
        }

        public static string UnBitShift(this string text)
        {
            int shft = 5;
            string decrypted = Encoding.UTF8.GetString(Convert.FromBase64String(text)).Select(ch => ch >> shft).Aggregate("", (current, val) => current + (char)(val / 2));
            return decrypted;
        }
    }
}
