using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DeusaldSharp
{
    public static class ZipExtensions
    {
        public static void CreateEntryFromAny(this ZipArchive archive, string sourceName, string entryName = "")
        {
            string fileName = Path.GetFileName(sourceName);
            if (File.GetAttributes(sourceName).HasFlag(FileAttributes.Directory))
            {
                archive.CreateEntryFromDirectory(sourceName, Path.Combine(entryName, fileName));
            }
            else
            {
                archive.CreateEntryFromFile(sourceName, Path.Combine(entryName, fileName), CompressionLevel.Fastest);
            }
        }

        public static void CreateEntryFromDirectory(this ZipArchive archive, string sourceDirName, string entryName = "")
        {
            string[] files = Directory.GetFiles(sourceDirName).Concat(Directory.GetDirectories(sourceDirName)).ToArray();
            foreach (var file in files)
            {
                archive.CreateEntryFromAny(file, entryName);
            }
        }
    }
}