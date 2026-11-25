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

using System.Drawing;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    /// <summary>
    /// Provides ANSI escape code constants and helpers for colorizing console output.
    /// Supports standard 16-color foreground and background constants, bright foreground
    /// variants, and 24-bit true-color methods for both foreground and background.
    /// Always append <see cref="RESET"/> after colored text to prevent color bleed into
    /// subsequent output. Requires a terminal with ANSI/VT100 support (Windows Terminal,
    /// macOS Terminal, most Linux terminals). On Windows, virtual terminal processing
    /// may need to be enabled for legacy console hosts.
    /// </summary>
    [PublicAPI]
    public static class Ansi
    {
        public const string RESET = "\x1b[0m";

        // Foreground colors
        public const string BLACK   = "\x1b[30m";
        public const string RED     = "\x1b[31m";
        public const string GREEN   = "\x1b[32m";
        public const string YELLOW  = "\x1b[33m";
        public const string BLUE    = "\x1b[34m";
        public const string MAGENTA = "\x1b[35m";
        public const string CYAN    = "\x1b[36m";
        public const string WHITE   = "\x1b[37m";

        // Background colors
        public const string BG_BLACK   = "\x1b[40m";
        public const string BG_RED     = "\x1b[41m";
        public const string BG_GREEN   = "\x1b[42m";
        public const string BG_YELLOW  = "\x1b[43m";
        public const string BG_BLUE    = "\x1b[44m";
        public const string BG_MAGENTA = "\x1b[45m";
        public const string BG_CYAN    = "\x1b[46m";
        public const string BG_WHITE   = "\x1b[47m";

        // Bright variants
        public const string BRIGHT_RED   = "\x1b[91m";
        public const string BRIGHT_GREEN = "\x1b[92m";
        public const string BRIGHT_BLUE  = "\x1b[94m";
        public const string BRIGHT_WHITE = "\x1b[97m";

        // 24-bit true color: \x1b[38;2;R;G;Bm for foreground
        //                    \x1b[48;2;R;G;Bm for background
        public static string Fg(int r, int g, int b) => $"\x1b[38;2;{r};{g};{b}m";
        public static string Bg(int r, int g, int b) => $"\x1b[48;2;{r};{g};{b}m";
        public static string Bg(Color color)         => Bg(color.R, color.G, color.B);
    }
}