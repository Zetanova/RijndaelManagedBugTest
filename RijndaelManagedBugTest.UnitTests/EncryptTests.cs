using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;
using Xunit.Abstractions;

namespace RijndaelManagedBugTest.UnitTests
{
    public sealed class EncryptTests
    {
        readonly ITestOutputHelper _output;

        const string secret = "testpass123testpass123testpass123";
        const int loopCount = 100;

        public EncryptTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void crypt_loop_empty_big_data()
        {
            var data = new byte[2630];

            for (int i = 0; i < loopCount; i++)
            {
                //PNetEncrypt.RandomBytes(data);
                var cipher = Array.Empty<byte>();

                try
                {
                    cipher = EncryptUtils.Encrypt(data, secret);

                    var r = EncryptUtils.Decrypt(cipher, secret);

                    Assert.True(data.SequenceEqual(r));
                }
                catch (CryptographicException ex) when (ex.Message == "Padding is invalid and cannot be removed.")
                {
                    //catch padding exception
                    _output.WriteLine("CryptographicException in loop[{0}] with empty data\ncipher: {1}",
                        i, Convert.ToBase64String(cipher));
                    throw;
                }
            }
        }

        //[Fact]
        //public void crypt_loop_random_big_data()
        //{
        //    var data = new byte[2630];

        //    for (int i = 0; i < loopCount; i++)
        //    {
        //        EncryptUtils.RandomBytes(data);

        //        var cipher = Array.Empty<byte>();

        //        try
        //        {
        //            cipher = EncryptUtils.Encrypt(data, secret);

        //            var r = EncryptUtils.Decrypt(cipher, secret);

        //            Assert.True(data.SequenceEqual(r));
        //        }
        //        catch (CryptographicException ex) when (ex.Message == "Padding is invalid and cannot be removed.")
        //        {
        //            //catch padding exception
        //            _output.WriteLine("CryptographicException in loop[{0}] with data: {1}\ncipher: {2}",
        //                i, Convert.ToBase64String(data), Convert.ToBase64String(cipher));
        //            throw;
        //        }
        //    }
        //}

        [Fact]
        public void decrypt_failed_cipher()
        {
            var secret = "testpass123testpass123testpass123";

            string cipherBase64 = "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";

            var cipher = Convert.FromBase64String(cipherBase64);
            var expected = new byte[2630];

            var data = Decrypt(cipher, secret);

            Assert.True(expected.SequenceEqual(data));


            static byte[] Decrypt(byte[] cipherBytes, string secret)
            {
                if (cipherBytes?.Length == 0)
                    return Array.Empty<byte>();

                var data = cipherBytes.AsSpan();

                if (data.Length <= 8)
                    throw new FormatException("cipher text invalid");

                var salt = data.Slice(0, 8).ToArray();
                var bytes = data.Slice(8).ToArray();

                var pdb = new Rfc2898DeriveBytes(secret, salt);

                var symmetricKey = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Key = pdb.GetBytes(32),
                    IV = pdb.GetBytes(16),
                    Padding = PaddingMode.PKCS7
                };

                var decryptor = symmetricKey.CreateDecryptor();

                var dms = new MemoryStream(bytes.Length);
                MemoryStream memoryStream = new MemoryStream(bytes);
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    cryptoStream.CopyTo(dms);

                return dms.ToArray();
            }
        }
    }
}
