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

using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    [PublicAPI]
    public static class ZipExtensions
    {
        public static void CreateEntryFromAny(this ZipArchive archive, string sourceName, string entryName = "")
        {
            string fileName = Path.GetFileName(sourceName);
            if (File.GetAttributes(sourceName).HasFlag(FileAttributes.Directory))
            {
                archive.CreateEntryFromDirectory(sourceName, CombineEntryName(entryName, fileName));
            }
            else
            {
                archive.CreateEntryFromFile(sourceName, CombineEntryName(entryName, fileName), CompressionLevel.Fastest);
            }
        }

        public static void CreateEntryFromDirectory(this ZipArchive archive, string sourceDirName, string entryName = "")
        {
            string[] files       = Directory.GetFiles(sourceDirName);
            string[] directories = Directory.GetDirectories(sourceDirName);

            // Keep empty directories in the archive as explicit "dir/" entries (the root itself needs no entry).
            if (files.Length == 0 && directories.Length == 0 && entryName.Length > 0)
            {
                archive.CreateEntry(NormalizeEntryName(entryName).TrimEnd('/') + "/");
                return;
            }

            foreach (string file in files) archive.CreateEntryFromAny(file, entryName);

            foreach (string directory in directories)
            {
                // Skip junctions / symbolic links to avoid infinite recursion and escaping the source tree.
                if (File.GetAttributes(directory).HasFlag(FileAttributes.ReparsePoint)) continue;
                archive.CreateEntryFromAny(directory, entryName);
            }
        }

        private static string CombineEntryName(string entryName, string name)
        {
            // Zip entry names always use '/' as separator, regardless of the OS.
            string normalized = NormalizeEntryName(entryName).TrimEnd('/');
            return normalized.Length == 0 ? name : normalized + "/" + name;
        }

        private static string NormalizeEntryName(string entryName)
        {
            return entryName.Replace('\\', '/');
        }
    }
}