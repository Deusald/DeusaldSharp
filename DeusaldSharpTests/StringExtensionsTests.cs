#nullable enable
using System;
using DeusaldSharp;
using NUnit.Framework;
// ReSharper disable VariableCanBeNotNullable

namespace DeusaldSharpTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        // --- ToSnakeCase ------------------------------------------------------

        [Test]
        public void ToSnakeCase_Null_Throws()
        {
            string? text = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.That(() => text!.ToSnakeCase(), Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("A", "a")]
        [TestCase("AB", "a_b")]
        [TestCase("HelloWorld", "hello_world")]
        [TestCase("helloWorld", "hello_world")]
        [TestCase("hello", "hello")]
        public void ToSnakeCase_ConvertsAsExpected(string input, string expected)
        {
            Assert.That(input.ToSnakeCase(), Is.EqualTo(expected));
        }

        // --- HashPassword / CompareHashedPasswords ----------------------------

        [Test]
        public void HashPassword_ReturnsBase64WithExpectedDecodedLength()
        {
            const string password = "P@ssw0rd!";
            string hash = password.HashPassword();

            byte[] bytes = Convert.FromBase64String(hash);

            // Based on your current constants: 16 salt + 32 hash = 48 bytes total.
            Assert.That(bytes.Length, Is.EqualTo(48));
        }

        [Test]
        public void HashPassword_SamePasswordTwice_ProducesDifferentHashes()
        {
            const string password = "P@ssw0rd!";
            string hash1 = password.HashPassword();
            string hash2 = password.HashPassword();

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void CompareHashedPasswords_CorrectPassword_ReturnsTrue()
        {
            const string password = "CorrectHorseBatteryStaple!";
            string stored = password.HashPassword();

            Assert.That(password.CompareHashedPasswords(stored), Is.True);
        }

        [Test]
        public void CompareHashedPasswords_WrongPassword_ReturnsFalse()
        {
            const string password = "P@ssw0rd!";
            const string wrong = "P@ssw0rd?";
            string stored = password.HashPassword();

            Assert.That(wrong.CompareHashedPasswords(stored), Is.False);
        }

        [Test]
        public void CompareHashedPasswords_InvalidBase64_ThrowsFormatException()
        {
            const string entered = "anything";
            const string invalid = "this is not base64!!!";

            Assert.That(() => entered.CompareHashedPasswords(invalid), Throws.TypeOf<FormatException>());
        }

        [Test]
        public void CompareHashedPasswords_ValidBase64ButWrongByteLength_ReturnsFalse()
        {
            const string entered = "anything";

            string tooShort = Convert.ToBase64String(new byte[10]);
            string tooLong  = Convert.ToBase64String(new byte[100]);

            Assert.That(entered.CompareHashedPasswords(tooShort), Is.False);
            Assert.That(entered.CompareHashedPasswords(tooLong), Is.False);
        }

        // --- ComputeHmacSha256Hex ---------------------------------------------

        [Test]
        public void ComputeHmacSha256Hex_KnownVector_MatchesExpected()
        {
            const string message = "The quick brown fox jumps over the lazy dog";
            const string key = "key";

            // Expected HMAC-SHA256 (hex lowercase) for ASCII message/key above.
            const string expected = "f7bc83f430538424b13298e6aa6fb143ef4d59a14946175997479dbc2d1a3cd8";

            Assert.That(message.ComputeHmacSha256Hex(key), Is.EqualTo(expected));
        }

        // --- Base64Encode / Base64Decode --------------------------------------

        [Test]
        public void Base64EncodeDecode_RoundTripsUtf8()
        {
            const string original = "Zażółć gęślą jaźń ✅";

            string encoded = original.Base64Encode();
            string decoded = encoded.Base64Decode();

            Assert.That(decoded, Is.EqualTo(original));
        }

        // --- FirstToUpper ------------------------------------------------------

        [TestCase("", "")]
        [TestCase("a", "A")]
        [TestCase("A", "A")]
        [TestCase("hello", "Hello")]
        public void FirstToUpper_Works(string input, string expected)
        {
            Assert.That(input.FirstToUpper(), Is.EqualTo(expected));
        }

        // --- SplitPascalCase ---------------------------------------------------

        [TestCase("HelloWorld", "Hello World")]
        [TestCase("JSONData", "JSON Data")]
        [TestCase("myURLParser", "my URL Parser")]
        [TestCase("A", "A")]
        [TestCase("", "")]
        public void SplitPascalCase_Works(string input, string expected)
        {
            Assert.That(input.SplitPascalCase(), Is.EqualTo(expected));
        }

        // --- IsNullOrWhiteSpace / IsNullOrEmpty / IsBlank / OrEmpty ------------

        [Test]
        public void IsNullOrWhiteSpace_Works()
        {
            string? a = null;
            string? b = "";
            string? c = "   ";
            string? d = "x";

            Assert.That(a.IsNullOrWhiteSpace(), Is.True);
            Assert.That(b.IsNullOrWhiteSpace(), Is.True);
            Assert.That(c.IsNullOrWhiteSpace(), Is.True);
            Assert.That(d.IsNullOrWhiteSpace(), Is.False);
        }

        [Test]
        public void IsNullOrEmpty_Works()
        {
            string? a = null;
            string? b = "";
            string? c = "   ";
            string? d = "x";

            Assert.That(a.IsNullOrEmpty(), Is.True);
            Assert.That(b.IsNullOrEmpty(), Is.True);
            Assert.That(c.IsNullOrEmpty(), Is.False);
            Assert.That(d.IsNullOrEmpty(), Is.False);
        }

        [Test]
        public void IsBlank_Works()
        {
            string? a = null;
            string? b = "";
            string? c = "   ";
            string? d = "x";

            Assert.That(a.IsBlank(), Is.True);
            Assert.That(b.IsBlank(), Is.True);
            Assert.That(c.IsBlank(), Is.True);
            Assert.That(d.IsBlank(), Is.False);
        }

        [Test]
        public void OrEmpty_Works()
        {
            string? a = null;
            string? b = "";
            string? c = "x";

            Assert.That(a.OrEmpty(), Is.EqualTo(string.Empty));
            Assert.That(b.OrEmpty(), Is.EqualTo(string.Empty));
            Assert.That(c.OrEmpty(), Is.EqualTo("x"));
        }
    }
}
