using System;

namespace DeusaldSharp.Analyzers;

public class DeusaldSharpAnalyzers
{
    /// <summary> Version of the generator, taken from Directory.Build.props at build time. </summary>
    public static readonly Version Version = CreateVersion();

    private static Version CreateVersion()
    {
        Version assemblyVersion = typeof(DeusaldSharpAnalyzers).Assembly.GetName().Version!;
        return new Version(assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
    }
}
