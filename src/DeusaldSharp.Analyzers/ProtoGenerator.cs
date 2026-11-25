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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DeusaldSharp.Analyzers;

[Generator]
[SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1036:Specify analyzer banned API enforcement setting")]
public sealed class ProtoGenerator : IIncrementalGenerator
{
    private const string _LIBRARY_NAMESPACE      = "DeusaldSharp";
    private const string _PROTO_MSG_BASE_NAME    = "ProtoMsgBase";
    private const string _PROTO_FIELD_ATTR_NAME  = "ProtoFieldAttribute";
    private const string _SERIALIZABLE_ENUM_ATTR = "SerializableEnumAttribute";
    private const string _HINT_SUFFIX            = ".DeusaldSharpProto.cs";

    private static readonly HashSet<string> _SpecialTypes =
    [
        "System.Guid", "System.DateTime", "System.TimeSpan", "System.Version", "System.Net.HttpStatusCode"
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // One model per message type: the transform only accepts the first declaration of a (partial) type,
        // so messages split across several partial declarations are generated exactly once.
        IncrementalValuesProvider<ProtoMessageModel> messages = context.SyntaxProvider
                                                                       .CreateSyntaxProvider(
                                                                            predicate: static (node, _) => node is ClassDeclarationSyntax,
                                                                            transform: static (ctx, ct) => CreateModel(ctx, ct))
                                                                       .Where(static m => m is not null)
                                                                       .Select(static (m, _) => m!);

        context.RegisterSourceOutput(messages, static (spc, model) => Emit(spc, model));
    }

    private static void Emit(SourceProductionContext spc, ProtoMessageModel model)
    {
        foreach (DiagnosticInfo diagnostic in model.Diagnostics)
            spc.ReportDiagnostic(ProtoDiagnostics.ToDiagnostic(diagnostic));

        if (!model.Generate) return;

        spc.AddSource(model.HintName, ProtoEmitter.Generate(model));
    }

    #region Model

    private static ProtoMessageModel? CreateModel(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        ClassDeclarationSyntax node = (ClassDeclarationSyntax)ctx.Node;

        if (ctx.SemanticModel.GetDeclaredSymbol(node, ct) is not INamedTypeSymbol msgType) return null;
        if (!IsFirstDeclaration(msgType, node)) return null;
        if (!DerivesFromProtoMsgBase(msgType)) return null;

        ct.ThrowIfCancellationRequested();

        List<DiagnosticInfo>  diagnostics = new List<DiagnosticInfo>();
        List<ProtoFieldModel> fields      = new List<ProtoFieldModel>();
        bool                  generate    = true;

        // Own fields: fully validated, problems reported here.
        Dictionary<int, IFieldSymbol> ownIds = new Dictionary<int, IFieldSymbol>();

        foreach (IFieldSymbol field in msgType.GetMembers().OfType<IFieldSymbol>())
        {
            AttributeData? attr = GetProtoFieldAttribute(field);
            if (attr is null) continue;

            ProtoFieldModel? model = CreateFieldModel(field, attr, false, diagnostics);

            if (model is null)
            {
                generate = false;
                continue;
            }

            if (ownIds.ContainsKey(model.Id))
            {
                diagnostics.Add(ProtoDiagnostics.Create(ProtoDiagnostics.DuplicateId, field.Locations.FirstOrDefault(), msgType.ToDisplayString(), model.Id));
                generate = false;
                continue;
            }

            ownIds.Add(model.Id, field);
            fields.Add(model);
        }

        // Inherited fields: problems of the base type itself are reported when the base type is processed,
        // here they only block generation. Conflicts with own fields and accessibility are reported here.
        Dictionary<int, IFieldSymbol> inheritedIds = new Dictionary<int, IFieldSymbol>();
        List<IFieldSymbol>            inaccessible = new List<IFieldSymbol>();
        Compilation                   compilation  = ctx.SemanticModel.Compilation;

        for (INamedTypeSymbol? baseType = msgType.BaseType; baseType is not null && !IsProtoMsgBase(baseType); baseType = baseType.BaseType)
        {
            foreach (IFieldSymbol field in baseType.GetMembers().OfType<IFieldSymbol>())
            {
                AttributeData? attr = GetProtoFieldAttribute(field);
                if (attr is null) continue;

                ProtoFieldModel? model = CreateFieldModel(field, attr, true, null);

                if (model is null || inheritedIds.ContainsKey(model.Id))
                {
                    generate = false;
                    continue;
                }

                inheritedIds.Add(model.Id, field);

                if (ownIds.TryGetValue(model.Id, out IFieldSymbol? ownField))
                {
                    diagnostics.Add(ProtoDiagnostics.Create(
                        ProtoDiagnostics.DuplicateInheritedId,
                        ownField.Locations.FirstOrDefault(),
                        $"{msgType.Name}.{ownField.Name}",
                        model.Id,
                        field.Name,
                        baseType.ToDisplayString()));
                    generate = false;
                    continue;
                }

                if (!IsAccessible(compilation, field, msgType)) inaccessible.Add(field);

                fields.Add(model);
            }
        }

        // Abstract messages are never instantiated: only their own fields are validated, the most derived
        // non-abstract message serializes the whole chain.
        if (msgType.IsAbstract)
            return CreateDiagnosticsOnlyModel(msgType, diagnostics);

        if (!IsPartial(msgType))
        {
            diagnostics.Add(ProtoDiagnostics.Create(ProtoDiagnostics.MustBePartial, msgType.Locations.FirstOrDefault(), msgType.ToDisplayString()));
            generate = false;
        }

        for (INamedTypeSymbol? container = msgType.ContainingType; container is not null; container = container.ContainingType)
        {
            if (IsPartial(container)) continue;

            diagnostics.Add(ProtoDiagnostics.Create(
                ProtoDiagnostics.ContainingTypeMustBePartial,
                container.Locations.FirstOrDefault(),
                container.ToDisplayString(),
                msgType.ToDisplayString()));
            generate = false;
        }

        foreach (IFieldSymbol field in inaccessible)
        {
            diagnostics.Add(ProtoDiagnostics.Create(
                ProtoDiagnostics.InaccessibleInheritedField,
                msgType.Locations.FirstOrDefault(),
                field.Name,
                field.ContainingType.ToDisplayString(),
                msgType.ToDisplayString()));
            generate = false;
        }

        fields.Sort((a, b) => a.Id.CompareTo(b.Id));

        return new ProtoMessageModel(
            HintName: CreateHintName(msgType),
            Namespace: msgType.ContainingNamespace.IsGlobalNamespace ? null : msgType.ContainingNamespace.ToDisplayString(),
            ContainingDeclarations: new EquatableArray<string>(GetContainingTypes(msgType).Select(CreateDeclaration)),
            Declaration: CreateDeclaration(msgType),
            FullyQualifiedName: msgType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            CanReset: CanReset(msgType),
            Generate: generate,
            Fields: new EquatableArray<ProtoFieldModel>(fields),
            Diagnostics: new EquatableArray<DiagnosticInfo>(diagnostics));
    }

    private static ProtoMessageModel CreateDiagnosticsOnlyModel(INamedTypeSymbol msgType, List<DiagnosticInfo> diagnostics)
        => new(
            HintName: CreateHintName(msgType),
            Namespace: null,
            ContainingDeclarations: EquatableArray<string>.Empty,
            Declaration: "",
            FullyQualifiedName: "",
            CanReset: false,
            Generate: false,
            Fields: EquatableArray<ProtoFieldModel>.Empty,
            Diagnostics: new EquatableArray<DiagnosticInfo>(diagnostics));

    private static bool IsFirstDeclaration(INamedTypeSymbol type, SyntaxNode node)
    {
        SyntaxReference first = type.DeclaringSyntaxReferences[0];
        return first.SyntaxTree == node.SyntaxTree && first.Span == node.Span;
    }

    private static bool IsProtoMsgBase(INamedTypeSymbol type)
        => type.Name == _PROTO_MSG_BASE_NAME && IsInLibraryNamespace(type);

    private static bool IsInLibraryNamespace(ISymbol symbol)
        => symbol.ContainingNamespace is { Name: _LIBRARY_NAMESPACE, ContainingNamespace.IsGlobalNamespace: true };

    private static bool DerivesFromProtoMsgBase(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? t = type.BaseType; t is not null; t = t.BaseType)
        {
            if (IsProtoMsgBase(t)) return true;
        }

        return false;
    }

    private static bool IsPartial(INamedTypeSymbol type)
        => type.DeclaringSyntaxReferences
               .Select(r => r.GetSyntax())
               .OfType<TypeDeclarationSyntax>()
               .Any(s => s.Modifiers.Any(SyntaxKind.PartialKeyword));

    private static bool IsAccessible(Compilation compilation, IFieldSymbol field, INamedTypeSymbol msgType)
    {
        if (field.AssociatedSymbol is IPropertySymbol property)
        {
            return property.GetMethod is not null
                && property.SetMethod is not null
                && compilation.IsSymbolAccessibleWithin(property.GetMethod, msgType)
                && compilation.IsSymbolAccessibleWithin(property.SetMethod, msgType);
        }

        return compilation.IsSymbolAccessibleWithin(field, msgType);
    }

    private static bool CanReset(INamedTypeSymbol msgType)
    {
        if (!msgType.InstanceConstructors.Any(c => c.Parameters.Length == 0)) return false;

        for (INamedTypeSymbol? t = msgType; t is not null; t = t.BaseType)
        {
            foreach (ISymbol member in t.GetMembers())
            {
                if (member is IFieldSymbol { IsRequired: true } or IPropertySymbol { IsRequired: true }) return false;
            }
        }

        return true;
    }

    private static IEnumerable<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol type)
    {
        Stack<INamedTypeSymbol> stack = new Stack<INamedTypeSymbol>();
        for (INamedTypeSymbol? t = type.ContainingType; t is not null; t = t.ContainingType) stack.Push(t);
        return stack;
    }

    private static string CreateDeclaration(INamedTypeSymbol type)
    {
        string keyword = type.TypeKind switch
        {
            TypeKind.Struct    => type.IsRecord ? "record struct" : "struct",
            TypeKind.Interface => "interface",
            _                  => type.IsRecord ? "record" : "class"
        };

        StringBuilder sb = new StringBuilder();
        sb.Append("partial ").Append(keyword).Append(' ').Append(EscapeIdentifier(type.Name));

        if (type.TypeParameters.Length > 0)
            sb.Append('<').Append(string.Join(", ", type.TypeParameters.Select(p => EscapeIdentifier(p.Name)))).Append('>');

        return sb.ToString();
    }

    private static string CreateHintName(INamedTypeSymbol type)
    {
        // Namespace + containing types + arity: unique per type within the compilation.
        StringBuilder name = new StringBuilder();

        if (!type.ContainingNamespace.IsGlobalNamespace)
            name.Append(type.ContainingNamespace.ToDisplayString()).Append('.');

        foreach (INamedTypeSymbol container in GetContainingTypes(type))
            name.Append(container.MetadataName).Append('+');

        name.Append(type.MetadataName);

        StringBuilder sanitized = new StringBuilder(name.Length + _HINT_SUFFIX.Length);

        foreach (char c in name.ToString())
        {
            sanitized.Append(c switch
            {
                _ when char.IsLetterOrDigit(c) => c,
                '.' or '_' or '-'              => c,
                '+'                            => '.',
                _                              => '_'
            });
        }

        return sanitized.Append(_HINT_SUFFIX).ToString();
    }

    internal static string EscapeIdentifier(string name)
        => SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ? "@" + name : name;

    #endregion Model

    #region Fields

    private static AttributeData? GetProtoFieldAttribute(IFieldSymbol field)
        => field.GetAttributes().FirstOrDefault(a => a.AttributeClass is { Name: _PROTO_FIELD_ATTR_NAME } attrClass && IsInLibraryNamespace(attrClass));

    /// <summary> Validates a [ProtoField] field. Returns null when invalid; diagnostics are added only when a list is passed. </summary>
    private static ProtoFieldModel? CreateFieldModel(IFieldSymbol field, AttributeData attr, bool inherited, List<DiagnosticInfo>? diagnostics)
    {
        Location? location    = field.Locations.FirstOrDefault();
        string    displayName = $"{field.ContainingType.Name}.{field.Name}";

        if (attr.ConstructorArguments.Length != 1 || attr.ConstructorArguments[0].Value is not ushort id || id == 0)
        {
            diagnostics?.Add(ProtoDiagnostics.Create(ProtoDiagnostics.InvalidId, location, displayName));
            return null;
        }

        if (field.IsStatic || field.IsConst)
        {
            diagnostics?.Add(ProtoDiagnostics.Create(ProtoDiagnostics.StaticField, location, displayName));
            return null;
        }

        // [field: ProtoField(n)] on an auto-property targets its backing field: access the property instead.
        string memberName = field.Name;

        if (field.AssociatedSymbol is IPropertySymbol property)
        {
            memberName = property.Name;

            if (property.SetMethod is null || property.SetMethod.IsInitOnly)
            {
                diagnostics?.Add(ProtoDiagnostics.Create(ProtoDiagnostics.ReadOnlyField, location, $"{field.ContainingType.Name}.{memberName}"));
                return null;
            }
        }
        else if (field.IsReadOnly)
        {
            diagnostics?.Add(ProtoDiagnostics.Create(ProtoDiagnostics.ReadOnlyField, location, displayName));
            return null;
        }

        ITypeSymbol type = field.Type;

        if (!TryClassifyField(type, out ProtoFieldKind kind, out ITypeSymbol valueType, out ProtoValueKind valueKind))
        {
            diagnostics?.Add(ProtoDiagnostics.Create(ProtoDiagnostics.UnsupportedType, location, displayName, type.ToDisplayString()));
            return null;
        }

        if (valueKind == ProtoValueKind.Object && valueType is ITypeParameterSymbol { HasConstructorConstraint: false } typeParameter)
        {
            DiagnosticDescriptor descriptor = kind switch
            {
                ProtoFieldKind.List or ProtoFieldKind.NullableList   => ProtoDiagnostics.GenericListNeedsNew,
                ProtoFieldKind.Array or ProtoFieldKind.NullableArray => ProtoDiagnostics.GenericArrayNeedsNew,
                _                                                    => ProtoDiagnostics.GenericFieldNeedsNew
            };

            diagnostics?.Add(ProtoDiagnostics.Create(descriptor, location, displayName, typeParameter.Name));
            return null;
        }

        bool isScalar = kind is ProtoFieldKind.Primitive or ProtoFieldKind.Special;

        return new ProtoFieldModel(
            Id: id,
            Name: EscapeIdentifier(memberName),
            IsInherited: inherited,
            Display: $"{type.ToDisplayString()} {memberName}",
            Kind: kind,
            ValueKind: valueKind,
            ValueType: valueType.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            ReadMethod: GetReadMethod(valueType, valueKind),
            CollectionReadMethod: GetCollectionReadMethod(valueType, valueKind, kind),
            IsReferenceValue: isScalar && valueType.IsReferenceType);
    }

    private static bool TryClassifyField(ITypeSymbol type, out ProtoFieldKind kind, out ITypeSymbol valueType, out ProtoValueKind valueKind)
    {
        kind      = default;
        valueType = type;
        valueKind = default;

        // Nullable<T> (value type wrapper)
        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
        {
            valueType = nullable.TypeArguments[0];
            kind      = ProtoFieldKind.NullableValue;
            return TryClassifyValue(valueType, out valueKind) && valueKind != ProtoValueKind.Object;
        }

        bool isNullableReference = type.IsReferenceType && type.NullableAnnotation == NullableAnnotation.Annotated;

        if (TryGetListElement(type, out ITypeSymbol listElement))
        {
            valueType = listElement;
            kind      = isNullableReference ? ProtoFieldKind.NullableList : ProtoFieldKind.List;
            return TryClassifyValue(valueType, out valueKind);
        }

        if (type is IArrayTypeSymbol array)
        {
            valueType = array.ElementType;
            kind      = isNullableReference ? ProtoFieldKind.NullableArray : ProtoFieldKind.Array;
            return array.Rank == 1 && TryClassifyValue(valueType, out valueKind);
        }

        if (!TryClassifyValue(type, out valueKind)) return false;

        kind = valueKind switch
        {
            ProtoValueKind.Primitive        => ProtoFieldKind.Primitive,
            ProtoValueKind.Special          => ProtoFieldKind.Special,
            ProtoValueKind.SerializableEnum => ProtoFieldKind.SerializableEnum,
            _                               => isNullableReference ? ProtoFieldKind.NullableObject : ProtoFieldKind.Object
        };

        return true;
    }

    private static bool TryClassifyValue(ITypeSymbol type, out ProtoValueKind valueKind)
    {
        valueKind = default;

        if (TryMapPrimitive(type, out _))
        {
            valueKind = ProtoValueKind.Primitive;
            return true;
        }

        if (_SpecialTypes.Contains(GetFullName(type)))
        {
            valueKind = ProtoValueKind.Special;
            return true;
        }

        if (type.TypeKind == TypeKind.Enum && type.GetAttributes().Any(a => a.AttributeClass is { Name: _SERIALIZABLE_ENUM_ATTR } attrClass && IsInLibraryNamespace(attrClass)))
        {
            valueKind = ProtoValueKind.SerializableEnum;
            return true;
        }

        if (IsProtoMsgBaseDerived(type))
        {
            valueKind = ProtoValueKind.Object;
            return true;
        }

        return false;
    }

    private static string GetFullName(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol { TypeArguments.Length: 0 } named) return "";
        if (named.ContainingType is not null) return "";

        return named.ContainingNamespace.IsGlobalNamespace ? named.Name : $"{named.ContainingNamespace.ToDisplayString()}.{named.Name}";
    }

    private static bool TryMapPrimitive(ITypeSymbol type, out string readMethod)
    {
        readMethod = type.SpecialType switch
        {
            SpecialType.System_Boolean => "ReadBoolean",
            SpecialType.System_Byte    => "ReadByte",
            SpecialType.System_SByte   => "ReadSByte",
            SpecialType.System_Int16   => "ReadInt16",
            SpecialType.System_UInt16  => "ReadUInt16",
            SpecialType.System_Int32   => "ReadInt32",
            SpecialType.System_UInt32  => "ReadUInt32",
            SpecialType.System_Int64   => "ReadInt64",
            SpecialType.System_UInt64  => "ReadUInt64",
            SpecialType.System_Single  => "ReadSingle",
            SpecialType.System_Double  => "ReadDouble",
            SpecialType.System_Char    => "ReadChar",
            SpecialType.System_String  => "ReadString",
            _                          => ""
        };

        return readMethod.Length > 0;
    }

    private static string GetReadMethod(ITypeSymbol valueType, ProtoValueKind valueKind)
    {
        switch (valueKind)
        {
            case ProtoValueKind.Primitive:
                TryMapPrimitive(valueType, out string read);
                return read;
            case ProtoValueKind.Special:
                return "Read" + valueType.Name;
            default:
                return "";
        }
    }

    private static string GetCollectionReadMethod(ITypeSymbol elemType, ProtoValueKind valueKind, ProtoFieldKind kind)
    {
        string suffix;

        switch (kind)
        {
            case ProtoFieldKind.List:
            case ProtoFieldKind.NullableList:
                suffix = "List";
                break;
            case ProtoFieldKind.Array:
            case ProtoFieldKind.NullableArray:
                suffix = "Array";
                break;
            default:
                return "";
        }

        switch (valueKind)
        {
            case ProtoValueKind.Primitive:
            {
                string name = elemType.SpecialType switch
                {
                    SpecialType.System_Byte    => "Byte",
                    SpecialType.System_SByte   => "SByte",
                    SpecialType.System_Boolean => "Bool",
                    SpecialType.System_Int16   => "Short",
                    SpecialType.System_UInt16  => "UShort",
                    SpecialType.System_Int32   => "Int",
                    SpecialType.System_UInt32  => "UInt",
                    SpecialType.System_Int64   => "Long",
                    SpecialType.System_UInt64  => "ULong",
                    SpecialType.System_Single  => "Float",
                    SpecialType.System_Double  => "Double",
                    SpecialType.System_Char    => "Char",
                    _                          => "String"
                };

                return $"Read{name}{suffix}";
            }
            case ProtoValueKind.Special:
                return $"Read{elemType.Name}{suffix}";
            case ProtoValueKind.SerializableEnum:
                return $"ReadSerializableEnum{suffix}";
            default:
                return "";
        }
    }

    private static bool TryGetListElement(ITypeSymbol type, out ITypeSymbol elementType)
    {
        elementType = null!;

        if (type is not INamedTypeSymbol { Name: "List", TypeArguments.Length: 1 } named) return false;
        if (named.ContainingNamespace.ToDisplayString() != "System.Collections.Generic") return false;

        elementType = named.TypeArguments[0];
        return true;
    }

    private static bool IsProtoMsgBaseDerived(ITypeSymbol type)
    {
        // Support generic type parameters (e.g., T) by looking at constraints.
        // The constraint itself may be the abstract ProtoMsgBase: new() makes the argument concrete.
        if (type is ITypeParameterSymbol typeParameter)
        {
            return typeParameter.ConstraintTypes.Any(c => c is ITypeParameterSymbol
                                                              ? IsProtoMsgBaseDerived(c)
                                                              : c is INamedTypeSymbol n && (IsProtoMsgBase(n) || DerivesFromProtoMsgBase(n)));
        }

        // Abstract messages cannot be created while deserializing.
        if (type is not INamedTypeSymbol { IsAbstract: false } named) return false;

        return DerivesFromProtoMsgBase(named);
    }

    #endregion Fields
}
