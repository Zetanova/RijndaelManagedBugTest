using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RijndaelManagedBugTest
{
    public static class EncryptUtils
    {
        public static byte[] RandomBytes(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            using var rnd = RandomNumberGenerator.Create();
            var bytes = new byte[count];
            rnd.GetBytes(bytes);
            return bytes;
        }

        public static void RandomBytes(Span<byte> bytes)
        {
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(bytes);
        }

        static RijndaelManaged CreateRijndael(string password, byte[] salt)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if (salt?.Length == 0) throw new ArgumentNullException(nameof(salt));

            using var pdb = new Rfc2898DeriveBytes(password, salt);

            var symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Key = pdb.GetBytes(32),
                IV = pdb.GetBytes(16),
                Padding = PaddingMode.PKCS7
            };

            return symmetricKey;
        }

        public static byte[] Encrypt(byte[] bytes, string secret)
        {
            var salt = RandomBytes(8);
            bytes = Encrypt(bytes, secret, salt);

            var data = new byte[8 + bytes.Length];
            Array.Copy(salt, data, 8);
            Array.Copy(bytes, 0, data, 8, bytes.Length);

            return data;
        }

        public static byte[] Decrypt(byte[] cipherBytes, string secret)
        {
            if (cipherBytes?.Length == 0)
                return Array.Empty<byte>();

            var data = cipherBytes.AsSpan();

            if (data.Length <= 8)
                throw new FormatException("cipher text invalid");

            var salt = data.Slice(0, 8).ToArray();
            var bytes = data.Slice(8).ToArray();
            bytes = Decrypt(bytes, secret, salt);

            return bytes;
        }

        public static byte[] Encrypt(byte[] bytes, string password, byte[] salt)
        {
            using var symmetricKey = CreateRijndael(password, salt);

            using var encryptor = symmetricKey.CreateEncryptor();

            var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            return memoryStream.ToArray();
        }

        public static byte[] Decrypt(byte[] cipherBytes, string password, byte[] salt)
        {
            using var symmetricKey = CreateRijndael(password, salt);

            using var decryptor = symmetricKey.CreateDecryptor();

            var dms = new MemoryStream(cipherBytes.Length);
            var memoryStream = new MemoryStream(cipherBytes);
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                cryptoStream.CopyTo(dms);

            return dms.ToArray();
        }

        public static string EncryptString(string plainText, string secret, byte[] salt)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(plainText), secret, salt));
        }

        public static string DecryptString(string cipherText, string secret, byte[] salt)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(cipherText), secret, salt));
        }

        public static string EncryptString(string plainText, string secret)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(plainText), secret));
        }

        public static string DecryptString(string cipherText, string secret)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(cipherText), secret));
        }
    }
}
