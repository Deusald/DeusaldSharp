// MIT License

// DeusaldSharp:
// Copyright (c) 2020 Adam "Deusald" Orliński

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class UsernameVerificatorTests
    {
        [Test]
        [TestOf(nameof(UsernameVerificator.CheckUsernameLengthRequirement))]
        public void UsernameVerificator_CheckUsernameLengthRequirement_Boundaries()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 3,
                maxCharacters: 6,
                whitespaceRequirement: false,
                charactersRequirementRegex: @"A-Za-z0-9_"
            );

            // Act
            bool tooShort  = verificator.CheckUsernameLengthRequirement("ab");
            bool atMin     = verificator.CheckUsernameLengthRequirement("abc");
            bool atMax     = verificator.CheckUsernameLengthRequirement("abcdef");
            bool tooLong   = verificator.CheckUsernameLengthRequirement("abcdefg");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(tooShort);
                Assert.IsTrue(atMin);
                Assert.IsTrue(atMax);
                Assert.IsFalse(tooLong);
            });
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CheckUsernameWhitespaceRequirement))]
        public void UsernameVerificator_CheckUsernameWhitespaceRequirement_Empty_IsTrue()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 0,
                maxCharacters: 32,
                whitespaceRequirement: true,
                charactersRequirementRegex: @"A-Za-z "
            );

            // Act
            bool result = verificator.CheckUsernameWhitespaceRequirement("");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CheckUsernameWhitespaceRequirement))]
        public void UsernameVerificator_CheckUsernameWhitespaceRequirement_LeadingOrTrailingSpace_IsFalse()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 0,
                maxCharacters: 32,
                whitespaceRequirement: true,
                charactersRequirementRegex: @"A-Za-z "
            );

            // Act
            bool leading  = verificator.CheckUsernameWhitespaceRequirement(" Adam");
            bool trailing = verificator.CheckUsernameWhitespaceRequirement("Adam ");
            bool middle   = verificator.CheckUsernameWhitespaceRequirement("Ad am");
            bool noSpace  = verificator.CheckUsernameWhitespaceRequirement("Adam");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(leading);
                Assert.IsFalse(trailing);
                Assert.IsTrue(middle);
                Assert.IsTrue(noSpace);
            });
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CheckCharactersRequirement))]
        public void UsernameVerificator_CheckCharactersRequirement_AllowsOnlyProvidedCharacterClass()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 0,
                maxCharacters: 32,
                whitespaceRequirement: false,
                charactersRequirementRegex: @"A-Za-z0-9 _"
            );

            // Act
            bool okOne   = verificator.CheckCharactersRequirement("Adam_123");
            bool okTwo   = verificator.CheckCharactersRequirement("Adam Orlinski");
            bool badOne  = verificator.CheckCharactersRequirement("Adam!");
            bool badTwo  = verificator.CheckCharactersRequirement("Ądam"); // outside A-Za-z in this regex

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(okOne);
                Assert.IsTrue(okTwo);
                Assert.IsFalse(badOne);
                Assert.IsFalse(badTwo);
            });
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CheckUsernameRequirements))]
        public void UsernameVerificator_CheckUsernameRequirements_WhitespaceRequirementEnabled_FailsOnEdgeSpaces()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 3,
                maxCharacters: 20,
                whitespaceRequirement: true,
                charactersRequirementRegex: @"A-Za-z0-9 "
            );

            // Act
            bool ok        = verificator.CheckUsernameRequirements("Adam 123");
            bool badStart  = verificator.CheckUsernameRequirements(" Adam 123");
            bool badEnd    = verificator.CheckUsernameRequirements("Adam 123 ");
            bool badChar   = verificator.CheckUsernameRequirements("Adam#123");
            bool badLength = verificator.CheckUsernameRequirements("ab");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(ok);
                Assert.IsFalse(badStart);
                Assert.IsFalse(badEnd);
                Assert.IsFalse(badChar);
                Assert.IsFalse(badLength);
            });
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CheckUsernameRequirements))]
        public void UsernameVerificator_CheckUsernameRequirements_WhitespaceRequirementDisabled_IgnoresWhitespaceRule()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 1,
                maxCharacters: 20,
                whitespaceRequirement: false,
                charactersRequirementRegex: @"A-Za-z0-9 "
            );

            // Act
            bool leadingOk  = verificator.CheckUsernameRequirements(" Adam");
            bool trailingOk = verificator.CheckUsernameRequirements("Adam ");
            bool middleOk   = verificator.CheckUsernameRequirements("Ad am");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(leadingOk);
                Assert.IsTrue(trailingOk);
                Assert.IsTrue(middleOk);
            });
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CleanUsername))]
        public void UsernameVerificator_CleanUsername_RemovesInvalidCharacters_AndTrims()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 0,
                maxCharacters: 64,
                whitespaceRequirement: true,
                charactersRequirementRegex: @"A-Za-z0-9 _"
            );

            // Act
            string cleaned = verificator.CleanUsername("  Ad!am@@ 12_3  ");

            // Assert
            // - invalid: ! @ @ removed
            // - leading/trailing whitespace trimmed
            Assert.AreEqual("Adam 12_3", cleaned);
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CleanUsername))]
        public void UsernameVerificator_CleanUsername_TruncatesToMaxCharacters()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 0,
                maxCharacters: 5,
                whitespaceRequirement: false,
                charactersRequirementRegex: @"A-Za-z"
            );

            // Act
            string cleaned = verificator.CleanUsername("   ABCDEFG   ");

            // Assert
            Assert.AreEqual("ABCDE", cleaned);
        }

        [Test]
        [TestOf(nameof(UsernameVerificator.CleanUsername))]
        public void UsernameVerificator_CleanUsername_AllInvalid_BecomesEmptyString()
        {
            // Arrange
            UsernameVerificator verificator = new UsernameVerificator(
                minCharacters: 0,
                maxCharacters: 32,
                whitespaceRequirement: false,
                charactersRequirementRegex: @"A-Za-z"
            );

            // Act
            string cleaned = verificator.CleanUsername("!!!@@@ 123");

            // Assert
            Assert.AreEqual("", cleaned);
        }
    }
}
