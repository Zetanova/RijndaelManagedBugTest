using System;
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
        const int loopCount = 10_000;

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

        [Fact]
        public void crypt_loop_random_big_data()
        {
            var data = new byte[2630];

            for (int i = 0; i < loopCount; i++)
            {
                EncryptUtils.RandomBytes(data);

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
                    _output.WriteLine("CryptographicException in loop[{0}] with data: {1}\ncipher: {2}",
                        i, Convert.ToBase64String(data), Convert.ToBase64String(cipher));
                    throw;
                }
            }
        }
    }
}
