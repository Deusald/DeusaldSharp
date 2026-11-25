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

using System.Text.RegularExpressions;

namespace DeusaldSharp
{
    public class UsernameVerificator
    {
        private readonly int    _MinCharacters;
        private readonly int    _MaxCharacters;
        private readonly bool   _WhitespaceRequirement;
        private readonly string _CharactersRequirementRegex; // For example @"A-Za-z0-9 _"

        public UsernameVerificator(int minCharacters, int maxCharacters, bool whitespaceRequirement, string charactersRequirementRegex)
        {
            _MinCharacters              = minCharacters;
            _MaxCharacters              = maxCharacters;
            _WhitespaceRequirement      = whitespaceRequirement;
            _CharactersRequirementRegex = charactersRequirementRegex;
        }

        public bool CheckUsernameRequirements(string username)
        {
            return CheckUsernameLengthRequirement(username) && (!_WhitespaceRequirement || CheckUsernameWhitespaceRequirement(username)) && CheckCharactersRequirement(username);
        }

        public bool CheckUsernameLengthRequirement(string username)
        {
            if (username.Length < _MinCharacters) return false;
            if (username.Length > _MaxCharacters) return false;
            return true;
        }

        public bool CheckUsernameWhitespaceRequirement(string username)
        {
            if (username.Length < 1) return true;
            if (username[0] == ' ') return false;
            if (username[^1] == ' ') return false;
            return true;
        }

        public bool CheckCharactersRequirement(string username)
        {
            return Regex.IsMatch(username, $"^[{_CharactersRequirementRegex}]+$");
        }

        public string CleanUsername(string username)
        {
            username = Regex.Replace(username, $"[^{_CharactersRequirementRegex}]+", "");
            username = username.Trim();
            return username.Length > _MaxCharacters ? username.Substring(0, _MaxCharacters) : username;
        }
    }
}