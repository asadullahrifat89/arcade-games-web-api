using System.Security.Cryptography;
using System.Text;

namespace AstroOdysseyCore.Extensions
{
    public static class StringExtensions
    {
        public const string NumericFormat = "#,0.##";

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

            string encryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(
                    password: encryptionKey,
                    salt: new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
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

            string encryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            encodedData = encodedData.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(encodedData);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(
                    password: encryptionKey,
                    salt: new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encodedData = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encodedData;
        }
    }
}
