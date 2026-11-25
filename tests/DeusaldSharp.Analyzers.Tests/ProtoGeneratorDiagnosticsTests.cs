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

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace DeusaldSharp.Analyzers.Tests;

public class ProtoGeneratorDiagnosticsTests
{
    // Minimal copy of the runtime contract the generated code compiles against.
    private const string _RUNTIME_STUB = @"
using System;
using System.IO;

namespace DeusaldSharp
{
    public abstract class ProtoMsgBase
    {
        public virtual void Serialize(BinaryWriter w) => throw new NotImplementedException();
        public virtual void Deserialize(BinaryReader r) => throw new NotImplementedException();
        protected virtual void DeserializeProtoFields(BinaryReader reader, long end) => throw new NotImplementedException();
        protected virtual void ResetProtoFields() { }
        protected static void DeserializeNested(ProtoMsgBase message, BinaryReader reader, long end) => message.DeserializeProtoFields(reader, end);
        protected static long BeginField(BinaryWriter writer, ushort id) => 0;
        protected static long BeginLengthPrefixed(BinaryWriter writer) => 0;
        protected static void EndLengthPrefixed(BinaryWriter writer, long start) { }
        protected static long ReadFieldHeader(BinaryReader reader, long end, out ushort id) { id = 0; return 0; }
        protected static void EndField(BinaryReader reader, ushort id, long fieldEnd) { }
        protected static int ReadCount(BinaryReader reader, long end, int minElementSize) => 0;
        protected static long ReadLengthPrefix(BinaryReader reader, long end) => 0;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ProtoFieldAttribute : Attribute
    {
        public ushort Id { get; }
        public ProtoFieldAttribute(ushort id) => Id = id;
    }
}
";

    private static (GeneratorRunResult Result, ImmutableArray<Diagnostic> CompileDiagnostics) Run(params string[] sources)
    {
        string trusted = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;

        CSharpCompilation compilation = CSharpCompilation.Create(
            "ProtoGeneratorDiagnosticsTests",
            new[] { _RUNTIME_STUB }.Concat(sources).Select((s, x) => CSharpSyntaxTree.ParseText(s, path: $"File{x}.cs")),
            trusted.Split(Path.PathSeparator).Select(p => MetadataReference.CreateFromFile(p)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new ProtoGenerator())
                                                      .RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out _);

        ImmutableArray<Diagnostic> compileDiagnostics = output.GetDiagnostics()
                                                              .Where(d => d.Severity == DiagnosticSeverity.Error)
                                                              .ToImmutableArray();

        return (driver.GetRunResult().Results.Single(), compileDiagnostics);
    }

    private static string[] GeneratorDiagnosticIds(GeneratorRunResult result)
        => result.Diagnostics.Select(d => d.Id).OrderBy(id => id).ToArray();

    [Fact]
    public void Valid_Messages_Compile_Without_Errors()
    {
        const string source = @"
using DeusaldSharp;

namespace A { public partial class Msg : ProtoMsgBase { [ProtoField(1)] public int X; } }
namespace B { public partial class Msg : ProtoMsgBase { [ProtoField(1)] public int X; } }

namespace C
{
    public partial class Gm : ProtoMsgBase { [ProtoField(1)] public int X; }
    public partial class Gm<T> : ProtoMsgBase { [ProtoField(1)] public int X; }

    public partial class Split : ProtoMsgBase { [ProtoField(1)] public int X; }
    public partial class Split { [ProtoField(2)] public int Y; }

    internal partial class Internal : ProtoMsgBase { [ProtoField(1)] public int @class; [ProtoField(2)] public string? @event; }

    public partial class Outer<T> { internal partial class Inner : ProtoMsgBase { [ProtoField(1)] public int X; } }

    public abstract class Base : ProtoMsgBase { [ProtoField(1)] protected int B; }
    public partial class Middle : Base { [ProtoField(2)] public int M; }
    public partial class Leaf : Middle { [ProtoField(3)] public int L; }

    public class Encoding { }
    public class MemoryStream { }
    public class List<T> { }
    public partial class Shadowed : ProtoMsgBase { [ProtoField(1)] public string S = """"; [ProtoField(2)] public Leaf? Child; [ProtoField(3)] public System.Collections.Generic.List<Leaf> Items = new(); }
}
";

        (GeneratorRunResult result, ImmutableArray<Diagnostic> errors) = Run(source, "namespace C { public partial class Split { [DeusaldSharp.ProtoField(3)] public int Z; } }");

        Assert.Null(result.Exception);
        Assert.Empty(result.Diagnostics);
        Assert.True(errors.IsEmpty, string.Join(Environment.NewLine, errors));

        string[] hintNames = result.GeneratedSources.Select(s => s.HintName).ToArray();
        Assert.Equal(hintNames.Length, hintNames.Distinct().Count());
        Assert.Single(hintNames, h => h.StartsWith("C.Split."));
        Assert.Contains("A.Msg.DeusaldSharpProto.cs", hintNames);
        Assert.Contains("B.Msg.DeusaldSharpProto.cs", hintNames);
        Assert.Contains("C.Gm_1.DeusaldSharpProto.cs", hintNames);
        Assert.Contains("C.Outer_1.Inner.DeusaldSharpProto.cs", hintNames);

        string leaf = result.GeneratedSources.Single(s => s.HintName == "C.Leaf.DeusaldSharpProto.cs").SourceText.ToString();
        Assert.Contains("base.B", leaf);
        Assert.Contains("base.M", leaf);
        Assert.Contains("this.L", leaf);
    }

    [Fact]
    public void Readonly_Field_Reports_DSHP008()
    {
        (GeneratorRunResult result, _) = Run("public partial class M : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] public readonly int X; }");
        Assert.Equal(["DSHP008"], GeneratorDiagnosticIds(result));
    }

    [Fact]
    public void GetOnly_Property_BackingField_Reports_DSHP008()
    {
        (GeneratorRunResult result, _) = Run("public partial class M : DeusaldSharp.ProtoMsgBase { [field: DeusaldSharp.ProtoField(1)] public int X { get; } }");
        Assert.Equal(["DSHP008"], GeneratorDiagnosticIds(result));
    }

    [Fact]
    public void Static_And_Const_Fields_Report_DSHP009()
    {
        (GeneratorRunResult result, _) = Run(
            "public partial class M : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] public static int X; [DeusaldSharp.ProtoField(2)] public const int Y = 1; }");
        Assert.Equal(["DSHP009", "DSHP009"], GeneratorDiagnosticIds(result));
    }

    [Fact]
    public void Duplicate_Id_Across_Inheritance_Reports_DSHP010()
    {
        (GeneratorRunResult result, _) = Run(@"
public abstract class B : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] public int X; }
public partial class D : B { [DeusaldSharp.ProtoField(1)] public int Y; }");

        Assert.Equal(["DSHP010"], GeneratorDiagnosticIds(result));
        Assert.Empty(result.GeneratedSources);
    }

    [Fact]
    public void Private_Base_Field_Reports_DSHP011()
    {
        (GeneratorRunResult result, _) = Run(@"
public abstract class B : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] private int X; }
public partial class D : B { [DeusaldSharp.ProtoField(2)] public int Y; }");

        Assert.Equal(["DSHP011"], GeneratorDiagnosticIds(result));
        Assert.Empty(result.GeneratedSources);
    }

    [Fact]
    public void Non_Partial_Containing_Type_Reports_DSHP012()
    {
        (GeneratorRunResult result, _) = Run("public class Outer { public partial class M : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] public int X; } }");

        Assert.Equal(["DSHP012"], GeneratorDiagnosticIds(result));
        Assert.Empty(result.GeneratedSources);
    }

    [Fact]
    public void Non_Partial_Message_Reports_DSHP001()
    {
        (GeneratorRunResult result, _) = Run("public class M : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] public int X; }");
        Assert.Equal(["DSHP001"], GeneratorDiagnosticIds(result));
    }

    [Fact]
    public void Nullable_Version_Is_Supported()
    {
        (GeneratorRunResult result, _) = Run("public partial class M : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] public System.Version? V; }");

        Assert.Empty(result.Diagnostics);
        Assert.Contains("if (this.V is not null)", result.GeneratedSources.Single().SourceText.ToString());
    }

    [Fact]
    public void Model_Is_Cached_When_Unrelated_Code_Changes()
    {
        const string message = "public partial class M : DeusaldSharp.ProtoMsgBase { [DeusaldSharp.ProtoField(1)] public int X; }";

        string             trusted    = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        MetadataReference[] references = trusted.Split(Path.PathSeparator).Select(p => (MetadataReference)MetadataReference.CreateFromFile(p)).ToArray();

        CSharpCompilation compilation = CSharpCompilation.Create(
            "Cache",
            [CSharpSyntaxTree.ParseText(_RUNTIME_STUB), CSharpSyntaxTree.ParseText(message), CSharpSyntaxTree.ParseText("public class Other { }")],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new ProtoGenerator().AsSourceGenerator()],
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));

        driver = driver.RunGenerators(compilation);

        CSharpCompilation changed = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees.Last(), CSharpSyntaxTree.ParseText("public class Other { public int Y; }"));
        driver = driver.RunGenerators(changed);

        GeneratorRunResult result = driver.GetRunResult().Results.Single();

        (object Value, IncrementalStepRunReason Reason)[] outputs = result.TrackedOutputSteps.SelectMany(s => s.Value).SelectMany(s => s.Outputs).ToArray();

        Assert.NotEmpty(outputs);
        Assert.All(outputs,
                   output => Assert.True(output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged, output.Reason.ToString()));
    }
}
