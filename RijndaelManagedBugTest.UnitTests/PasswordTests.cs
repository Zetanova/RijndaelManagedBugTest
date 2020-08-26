using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RijndaelManagedBugTest.UnitTests
{
    public class PasswordTests
    {
        [Fact]
        public void EncryptPassword()
        {
            var secret = Password.Generate();

            Assert.NotEmpty(secret);

            var plainText = "secret#8Test1";
            var salt = EncryptUtils.RandomBytes(8);

            var cipherText = EncryptUtils.EncryptString(plainText, secret, salt);

            var result = EncryptUtils.DecryptString(cipherText, secret, salt);

            Assert.Equal(plainText, result);
        }

        [Fact]
        public void EncryptSecret()
        {
            var secret = Password.Generate();

            Assert.NotEmpty(secret);

            var plainText = "secret#8Test2";

            var cipherText = EncryptUtils.EncryptString(plainText, secret);

            var result = EncryptUtils.DecryptString(cipherText, secret);

            Assert.Equal(plainText, result);
        }
    }
}
