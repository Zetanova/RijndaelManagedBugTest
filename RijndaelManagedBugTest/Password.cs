using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RijndaelManagedBugTest
{
    public sealed class PasswordOptions
    {
        public static readonly PasswordOptions Default = new PasswordOptions
        {
            MinLength = 8,
            MaxLength = 10,
            MinUpper = 1,
            MaxUpper = 2,
            MinNumbers = 1,
            MaxNumbers = 2,
            MinSpecial = 1,
            MaxSpecial = 1,
            AllowSpecialStart = false
        };

        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public int MinUpper { get; set; }
        public int MaxUpper { get; set; }

        public int MinNumbers { get; set; }
        public int MaxNumbers { get; set; }

        public int MinSpecial { get; set; }
        public int MaxSpecial { get; set; }

        public bool AllowSpecialStart { get; set; }

        public bool IsValid(string password)
        {
            if (!(MinLength <= password.Length && password.Length <= MaxLength))
                return false;

            if (!AllowSpecialStart && !char.IsLetterOrDigit(password, 0))
                return false;

            int u = 0;
            int n = 0;
            int s = 0;

            char c;
            for (int i = 0; i < password.Length; i++)
            {
                c = password[i];
                if (char.IsUpper(c))
                    u++;
                else if (char.IsNumber(c))
                    n++;
                else if (!char.IsLower(c))
                    s++;
            }

            return (MinUpper <= u && u <= MaxUpper)
                && (MinNumbers <= n && n <= MaxNumbers)
                && (MinSpecial <= s && s <= MaxSpecial);
        }
    }

    public static class Password
    {
        public const string SpecialCharacters = "!#+&%";

        public const string AlphaNumericCharacters =
            "ABCDEFGHJKLMNPRSTUVWX" +
            "abcdefghijkmnpqrstuvwx" +
            "23456789";

        public const string PasswordCharacters = AlphaNumericCharacters + SpecialCharacters;

        public static string Generate()
        {
            return Generate(PasswordOptions.Default);
        }

        public static string Generate(PasswordOptions options)
        {
            using var gen = new RNGCryptoServiceProvider();
            Encoding enc = Encoding.ASCII;

            var randomBytes = new byte[options.MaxLength];
            var randomByte = new byte[1];

            char[] chars;

            var isValid = false;
            string password = null;

            do
            {
                gen.GetNonZeroBytes(randomBytes);
                chars = enc.GetChars(randomBytes);

                for (int i = 0; i < chars.Length; i++)
                {
                    //if (!Char.IsLetter(chars[i]) && !Char.IsNumber(chars[i]))
                    if (PasswordCharacters.IndexOf(chars[i]) < 0)
                    {
                        gen.GetNonZeroBytes(randomByte);
                        enc.GetChars(randomByte, 0, 1, chars, i);
                        i--;
                    }
                }

                for (int i = options.MinLength; i <= options.MaxLength; i++)
                {
                    password = new string(chars, 0, i);
                    isValid = options.IsValid(password);
                    if (isValid)
                        break;
                }
            }
            while (!isValid);

            //Debug.WriteLine(String.Format("Password '{0}' generiert.", password), "DEBUG");
            return password;
        }
    }
}
