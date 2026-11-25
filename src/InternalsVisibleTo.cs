// Shared by every module through src/Directory.Build.props.
// A real source file (instead of the <InternalsVisibleTo> item that MSBuild generates into obj/) so IDEs always see it.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DeusaldSharpTests")]
