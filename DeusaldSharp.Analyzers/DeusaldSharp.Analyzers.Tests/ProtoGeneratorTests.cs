using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace DeusaldSharp.Analyzers.Tests;

public class ProtoGeneratorTests
{
    private const string _MESSAGE_SOURCE = @"
using System;
using System.IO;

namespace DeusaldSharp
{
    public abstract class ProtoMsgBase
    {
        public virtual void Serialize(BinaryWriter w) => throw new NotImplementedException();
        public virtual void Deserialize(BinaryReader r) => throw new NotImplementedException();
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ProtoFieldAttribute : Attribute
    {
        public ushort Id { get; }
        public ProtoFieldAttribute(ushort id) => Id = id;
    }
}

namespace MyGame
{
    public partial class ChildMsg : DeusaldSharp.ProtoMsgBase
    {
        [DeusaldSharp.ProtoField(1)] public int X;
        [DeusaldSharp.ProtoField(2)] public bool Flag;
    }
}
";

    [Fact]
    public void Generates_Model_And_Overrides()
    {
        ProtoGenerator        generator = new ProtoGenerator(); // or ProtoGenerator, whatever your class name is
        CSharpGeneratorDriver driver    = CSharpGeneratorDriver.Create(generator);

        CSharpCompilation compilation = CSharpCompilation.Create(
            nameof(ProtoGeneratorTests),
            [CSharpSyntaxTree.ParseText(_MESSAGE_SOURCE)],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.IO.BinaryWriter).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriverRunResult runResult = driver.RunGenerators(compilation).GetRunResult();

        // If nothing was generated, print hint names to make debugging easy.
        Assert.NotEmpty(runResult.Results);

        GeneratorRunResult result = runResult.Results.Single(); // one generator
        Assert.NotEmpty(result.GeneratedSources);

        // Debug helper: list all hint names in the failure message if we can't find the expected one
        string hintNames = string.Join(", ", result.GeneratedSources.Select(s => s.HintName));

        // Find the generated source by hint name (this is what AddSource uses)
        GeneratedSourceResult generated = result.GeneratedSources
                                                .FirstOrDefault(s => s.HintName.Contains("ChildMsg", StringComparison.OrdinalIgnoreCase));

        Assert.True(generated.HintName.Length > 0,
            $"No generated source matched 'ChildMsg'. Generated hint names: {hintNames}");

        string text = generated.SourceText.ToString();
        
        Assert.Contains("partial class ChildMsg",           text);
        Assert.Contains("public override void Serialize",   text);
        Assert.Contains("public override void Deserialize", text);
    }
}