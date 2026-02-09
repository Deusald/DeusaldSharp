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

using JetBrains.Annotations;

namespace DeusaldSharp
{
    /// <summary> Logging levels. </summary>
    [PublicAPI]
    public enum LogLevel
    {
        /// <summary> Don't print log. </summary>
        Off,
            
        /// <summary> Very detailed logs, which may include high-volume information such as protocol payloads.
        /// This log level is typically only enabled during development. </summary>
        Trace,
            
        /// <summary> Debugging information, less detailed than trace, typically not enabled in production environment. </summary>
        Debug,
            
        /// <summary> Information messages, which are normally enabled in production environment. </summary>
        Info,
            
        /// <summary> Warning messages, typically for non-critical issues, which can be recovered or which are temporary failures. </summary>
        Warn,
            
        /// <summary> Error messages - most of the time these are Exceptions. </summary>
        Error,
            
        /// <summary> Very serious errors! </summary>
        Fatal
    }
}