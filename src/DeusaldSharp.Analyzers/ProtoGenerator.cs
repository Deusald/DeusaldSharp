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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DeusaldSharp.Analyzers;

[Generator]
[SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1036:Specify analyzer banned API enforcement setting")]
public sealed class ProtoGenerator : IIncrementalGenerator
{
    private const string _PROTO_MSG_BASE_FULL_NAME   = "DeusaldSharp.ProtoMsgBase";
    private const string _PROTO_FIELD_ATTR_FULL_NAME = "DeusaldSharp.ProtoFieldAttribute";
    private const string _SERIALIZABLE_ENUM_ATTR     = "DeusaldSharp.SerializableEnumAttribute";

    private static readonly HashSet<string> _SpecialTypes =
    [
        "System.Guid", "System.DateTime", "System.TimeSpan", "System.Version", "System.Net.HttpStatusCode"
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<INamedTypeSymbol?> baseTypeSymbol = context.CompilationProvider
                                                                            .Select((compilation, _) => compilation.GetTypeByMetadataName(_PROTO_MSG_BASE_FULL_NAME));

        IncrementalValuesProvider<INamedTypeSymbol?> candidateTypes = context.SyntaxProvider.CreateSyntaxProvider(
                                                                                  predicate: static (node, _) => node is ClassDeclarationSyntax,
                                                                                  transform: static (ctx, _) =>
                                                                                      (INamedTypeSymbol?)ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node))
                                                                             .Where(static s => s is not null);

        IncrementalValuesProvider<INamedTypeSymbol> messages = candidateTypes
                                                              .Combine(baseTypeSymbol)
                                                              .Where(static pair => pair.Right is not null && DerivesFrom(pair.Left!, pair.Right!))
                                                              .Select(static (pair, _) => pair.Left!);

        context.RegisterSourceOutput(messages, static (spc, symbol) => Emit(spc, symbol));
    }

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (INamedTypeSymbol? t = type; t is not null; t = t.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(t, baseType))
                return true;
        }
        return false;
    }

    private static void Emit(SourceProductionContext spc, INamedTypeSymbol msgType)
    {
        if (IsAbstract(msgType)) return;

        if (!IsPartial(msgType))
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "DSHP001",
                    title: "Proto message must be partial",
                    messageFormat: "Type '{0}' derives from ProtoMsgBase but is not partial. Mark it 'partial' so code can be generated.",
                    category: "DeusaldSharp.Proto",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                msgType.Locations.FirstOrDefault(),
                msgType.ToDisplayString()));
            return;
        }

        List<FieldInfo>? fields = GatherFields(spc, msgType);
        if (fields is null) return;

        string src = Generate(msgType, fields);
        spc.AddSource($"{msgType.Name}.DeusaldSharpProto.g.cs", src);
    }

    private static bool IsAbstract(INamedTypeSymbol t)
        => t.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .Any(s => s.Modifiers.Any(m => m.Text == "abstract"));

    private static bool IsPartial(INamedTypeSymbol t)
        => t.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .Any(s => s.Modifiers.Any(m => m.Text == "partial"));

    private sealed record FieldInfo(ushort Id, string Name, ITypeSymbol Type, NullableAnnotation NullableAnnotation)
    {
        public ushort             Id                 { get; } = Id;
        public string             Name               { get; } = Name;
        public ITypeSymbol        Type               { get; } = Type;
        public NullableAnnotation NullableAnnotation { get; } = NullableAnnotation;
    }

    private static List<FieldInfo>? GatherFields(SourceProductionContext spc, INamedTypeSymbol msgType)
    {
        List<FieldInfo> list = new List<FieldInfo>();
        HashSet<ushort> seen = new HashSet<ushort>();

        foreach (IFieldSymbol f in msgType.GetMembers().OfType<IFieldSymbol>())
        {
            if (f.IsStatic) continue;

            AttributeData? attr = f.GetAttributes().FirstOrDefault(a =>
                a.AttributeClass?.ToDisplayString() == _PROTO_FIELD_ATTR_FULL_NAME);

            if (attr is null) continue;

            if (attr.ConstructorArguments.Length != 1 ||
                attr.ConstructorArguments[0].Value is not ushort id ||
                id == 0)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DSHP002",
                        title: "Invalid ProtoField id",
                        messageFormat: "Field '{0}' has an invalid [ProtoField] id. Id must be a non-zero ushort.",
                        category: "DeusaldSharp.Proto",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    f.Locations.FirstOrDefault(),
                    $"{msgType.Name}.{f.Name}"));
                return null;
            }

            if (!seen.Add(id))
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DSHP003",
                        title: "Duplicate ProtoField id",
                        messageFormat: "Type '{0}' contains duplicate [ProtoField({1})]. Field ids must be unique per message.",
                        category: "DeusaldSharp.Proto",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    f.Locations.FirstOrDefault(),
                    msgType.ToDisplayString(),
                    id));
                return null;
            }

            // Direct generic field: public T Data;
            if (f.Type is ITypeParameterSymbol tpField && IsProtoMsgBaseDerived(tpField) && !tpField.HasConstructorConstraint)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DSHP005",
                        title: "Generic Proto field type must have new() constraint",
                        messageFormat:
                        "Field '{0}' uses generic type parameter '{1}' which is deserialized via 'new {1}()'. Add 'where {1} : new()' to the containing type.",
                        category: "DeusaldSharp.Proto",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    f.Locations.FirstOrDefault(),
                    $"{msgType.Name}.{f.Name}",
                    tpField.Name));
                return null;
            }

            // List generic element: public List<T> Data;
            if (TryGetListElement(f.Type, out ITypeSymbol elem) &&
                elem is ITypeParameterSymbol tpElem &&
                IsProtoMsgBaseDerived(tpElem) &&
                !tpElem.HasConstructorConstraint)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DSHP006",
                        title: "Generic List Proto field type must have new() constraint",
                        messageFormat:
                        "Field '{0}' uses generic type parameter '{1}' which is deserialized via 'new {1}()'. Add 'where {1} : new()' to the containing type.",
                        category: "DeusaldSharp.Proto",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    f.Locations.FirstOrDefault(),
                    $"{msgType.Name}.{f.Name}",
                    tpElem.Name));
                return null;
            }

            // Array generic element: public T[] Data;
            if (TryGetArrayElement(f.Type, out ITypeSymbol arrElem) &&
                arrElem is ITypeParameterSymbol tpArrElem &&
                IsProtoMsgBaseDerived(tpArrElem) &&
                !tpArrElem.HasConstructorConstraint)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DSHP007",
                        title: "Generic Array Proto field type must have new() constraint",
                        messageFormat:
                        "Field '{0}' uses generic type parameter '{1}' which is deserialized via 'new {1}()'. Add 'where {1} : new()' to the containing type.",
                        category: "DeusaldSharp.Proto",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    f.Locations.FirstOrDefault(),
                    $"{msgType.Name}.{f.Name}",
                    tpArrElem.Name));
                return null;
            }

            if (!TryClassifyFieldType(f.Type, f.NullableAnnotation, out _))
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DSHP004",
                        title: "Unsupported field type",
                        messageFormat: "Field '{0}' has unsupported type '{1}'.",
                        category: "DeusaldSharp.Proto",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    f.Locations.FirstOrDefault(),
                    $"{msgType.Name}.{f.Name}",
                    f.Type.ToDisplayString()));
                return null;
            }

            list.Add(new FieldInfo(id, f.Name, f.Type, f.NullableAnnotation));
        }

        list.Sort((a, b) => a.Id.CompareTo(b.Id));
        return list;
    }

    private enum FieldKind
    {
        Primitive,
        Special,
        SerializableEnum,

        NullableValue,  // Nullable<T> where T is primitive/special/enum
        Object,         // ProtoMsgBase-derived (non-null)
        NullableObject, // ProtoMsgBase-derived (nullable ref)
        List,           // List<T> where T is supported
        NullableList,   // nullable ref List<T> where T is supported
        Array,          // T[] where T is supported
        NullableArray,  // nullable ref T[] where T is supported
    }

    private sealed record FieldTypeInfo(FieldKind Kind, ITypeSymbol? ElementType = null, FieldTypeInfo? Inner = null)
    {
        public FieldKind      Kind        { get; } = Kind;
        public ITypeSymbol?   ElementType { get; } = ElementType;
        public FieldTypeInfo? Inner       { get; } = Inner;
    }

    private static bool TryClassifyFieldType(ITypeSymbol type, NullableAnnotation nullableAnn, out FieldTypeInfo info)
    {
        // Nullable<T> (value type wrapper)
        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, TypeArguments.Length: 1 } nts)
        {
            ITypeSymbol innerType = nts.TypeArguments[0];
            if (!TryClassifyNonNullable(innerType, out FieldTypeInfo inner))
            {
                info = null!;
                return false;
            }

            if (inner.Kind is FieldKind.Primitive or FieldKind.SerializableEnum or FieldKind.Special)
            {
                info = new FieldTypeInfo(FieldKind.NullableValue, Inner: inner);
                return true;
            }

            info = null!;
            return false;
        }

        // nullable reference types
        if (type.IsReferenceType && nullableAnn == NullableAnnotation.Annotated)
        {
            if (IsProtoMsgBaseDerived(type))
            {
                info = new FieldTypeInfo(FieldKind.NullableObject);
                return true;
            }

            if (TryGetListElement(type, out ITypeSymbol elem))
            {
                if (!TryClassifyListElement(elem))
                {
                    info = null!;
                    return false;
                }

                info = new FieldTypeInfo(FieldKind.NullableList, ElementType: elem);
                return true;
            }

            if (TryGetArrayElement(type, out ITypeSymbol arrElem))
            {
                if (!TryClassifyArrayElement(arrElem))
                {
                    info = null!;
                    return false;
                }

                info = new FieldTypeInfo(FieldKind.NullableArray, ElementType: arrElem);
                return true;
            }
        }

        return TryClassifyNonNullable(type, out info);
    }

    private static bool TryClassifyNonNullable(ITypeSymbol type, out FieldTypeInfo info)
    {
        if (TryMapPrimitive(type, out _))
        {
            info = new FieldTypeInfo(FieldKind.Primitive);
            return true;
        }

        string full = type.ToDisplayString();

        if (_SpecialTypes.Contains(full))
        {
            info = new(FieldKind.Special);
            return true;
        }

        if (type.TypeKind == TypeKind.Enum && HasAttribute(type, _SERIALIZABLE_ENUM_ATTR))
        {
            info = new(FieldKind.SerializableEnum);
            return true;
        }

        if (IsProtoMsgBaseDerived(type))
        {
            info = new(FieldKind.Object);
            return true;
        }

        if (TryGetListElement(type, out ITypeSymbol elemType))
        {
            if (!TryClassifyListElement(elemType))
            {
                info = null!;
                return false;
            }

            info = new(FieldKind.List, ElementType: elemType);
            return true;
        }

        if (TryGetArrayElement(type, out ITypeSymbol arrElemType))
        {
            if (!TryClassifyArrayElement(arrElemType))
            {
                info = null!;
                return false;
            }

            info = new(FieldKind.Array, ElementType: arrElemType);
            return true;
        }

        info = null!;
        return false;
    }

    private static bool TryClassifyListElement(ITypeSymbol elemType)
    {
        if (TryMapPrimitive(elemType, out _)) return true;

        string full = elemType.ToDisplayString();

        if (_SpecialTypes.Contains(full)) return true;

        if (elemType.TypeKind == TypeKind.Enum && HasAttribute(elemType, _SERIALIZABLE_ENUM_ATTR)) return true;

        if (IsProtoMsgBaseDerived(elemType)) return true;
        return false;
    }

    private static bool TryClassifyArrayElement(ITypeSymbol elemType)
    {
        if (TryMapPrimitive(elemType, out _)) return true;

        string full = elemType.ToDisplayString();

        if (_SpecialTypes.Contains(full)) return true;

        if (elemType.TypeKind == TypeKind.Enum && HasAttribute(elemType, _SERIALIZABLE_ENUM_ATTR)) return true;

        if (IsProtoMsgBaseDerived(elemType)) return true;
        return false;
    }

    private static bool TryMapPrimitive(ITypeSymbol type, out string readMethod)
    {
        readMethod = "";
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
                readMethod = "ReadBoolean";
                return true;
            case SpecialType.System_Byte:
                readMethod = "ReadByte";
                return true;
            case SpecialType.System_SByte:
                readMethod = "ReadSByte";
                return true;
            case SpecialType.System_Int16:
                readMethod = "ReadInt16";
                return true;
            case SpecialType.System_UInt16:
                readMethod = "ReadUInt16";
                return true;
            case SpecialType.System_Int32:
                readMethod = "ReadInt32";
                return true;
            case SpecialType.System_UInt32:
                readMethod = "ReadUInt32";
                return true;
            case SpecialType.System_Int64:
                readMethod = "ReadInt64";
                return true;
            case SpecialType.System_UInt64:
                readMethod = "ReadUInt64";
                return true;
            case SpecialType.System_Single:
                readMethod = "ReadSingle";
                return true;
            case SpecialType.System_Double:
                readMethod = "ReadDouble";
                return true;
            case SpecialType.System_Char:
                readMethod = "ReadChar";
                return true;
            case SpecialType.System_String:
                readMethod = "ReadString";
                return true;
            default: return false;
        }
    }

    private static bool HasAttribute(ITypeSymbol type, string attrFullName)
        => type.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == attrFullName);

    private static bool TryGetListElement(ITypeSymbol type, out ITypeSymbol elementType)
    {
        elementType = null!;

        if (type is not INamedTypeSymbol nts) return false;
        if (nts.TypeArguments.Length != 1) return false;

        // System.Collections.Generic.List<T>
        if (nts.Name != "List") return false;
        if (nts.ContainingNamespace.ToDisplayString() != "System.Collections.Generic") return false;

        elementType = nts.TypeArguments[0];
        return true;
    }

    private static bool TryGetArrayElement(ITypeSymbol type, out ITypeSymbol elementType)
    {
        elementType = null!;

        if (type is not IArrayTypeSymbol arrType) return false;

        // Only support single-dimensional arrays
        if (arrType.Rank != 1) return false;

        elementType = arrType.ElementType;
        return true;
    }

    private static bool IsProtoMsgBaseDerived(ITypeSymbol type)
    {
        // Support generic type parameters (e.g., T) by looking at constraints.
        if (type is ITypeParameterSymbol tp)
        {
            foreach (var c in tp.ConstraintTypes)
            {
                if (IsProtoMsgBaseDerived(c))
                    return true;
            }
            return false;
        }

        if (type is not INamedTypeSymbol nts) return false;

        for (INamedTypeSymbol? t = nts; t is not null; t = t.BaseType)
        {
            if (t.ToDisplayString() == _PROTO_MSG_BASE_FULL_NAME)
                return true;
        }
        return false;
    }

    private static string Generate(INamedTypeSymbol msgType, List<FieldInfo> fields)
    {
        string?       ns      = msgType.ContainingNamespace.IsGlobalNamespace ? null : msgType.ContainingNamespace.ToDisplayString();
        string        msgName = msgType.Name;
        StringBuilder sb      = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.IO;");
        sb.AppendLine("using System.Text;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using DeusaldSharp;");
        sb.AppendLine();

        if (ns is not null) sb.Append("namespace ").Append(ns).AppendLine(" {");

        sb.Append("public partial class ").Append(msgName);

        if (msgType.TypeParameters.Length > 0)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", msgType.TypeParameters.Select(p => p.Name)));
            sb.Append('>');
        }

        sb.AppendLine();

        sb.AppendLine("{");

        // Serialize
        sb.AppendLine("    public override void Serialize(BinaryWriter writer)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (!writer.BaseStream.CanSeek)");
        sb.AppendLine("            throw new InvalidOperationException(\"Proto serialization requires a seekable stream (e.g., MemoryStream). \");");
        sb.AppendLine();

        if (fields.Count > 0)
        {
            sb.AppendLine("        long __lenPos;");
            sb.AppendLine("        long __start;");
            sb.AppendLine("        long __end;");
            sb.AppendLine("        int __payloadLen;");
        }

        foreach (FieldInfo f in fields)
        {
            TryClassifyFieldType(f.Type, f.NullableAnnotation, out FieldTypeInfo info);
            EmitSerializeField(sb, f, info);
        }

        sb.AppendLine("    }");
        sb.AppendLine();

        // Deserialize
        sb.AppendLine("    public override void Deserialize(BinaryReader reader)");
        sb.AppendLine("    {");
        sb.AppendLine("        while (reader.BaseStream.Position < reader.BaseStream.Length)");
        sb.AppendLine("        {");
        sb.AppendLine("            ushort id     = reader.ReadUInt16();");
        sb.AppendLine("            int    length = reader.ReadInt32();");
        sb.AppendLine("            long   end    = reader.BaseStream.Position + length;");
        sb.AppendLine();
        sb.AppendLine("            switch (id)");
        sb.AppendLine("            {");

        foreach (FieldInfo f in fields)
        {
            TryClassifyFieldType(f.Type, f.NullableAnnotation, out FieldTypeInfo info);
            sb.AppendLine($"                case {f.Id}:");
            EmitDeserializeField(sb, f, info);
            sb.AppendLine("                    break;");
            sb.AppendLine();
        }

        sb.AppendLine("                default:");
        sb.AppendLine("                    reader.BaseStream.Seek(length, SeekOrigin.Current);");
        sb.AppendLine("                    break;");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            reader.BaseStream.Position = end;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");

        sb.AppendLine("}");

        if (ns is not null) sb.AppendLine("}");

        return sb.ToString();
    }

    private static void EmitSerializeField(StringBuilder sb, FieldInfo f, FieldTypeInfo info)
    {
        sb.AppendLine($"        // Field {f.Id}: {f.Type.ToDisplayString()} {f.Name}");
        sb.AppendLine($"        writer.Write((ushort){f.Id});");
        sb.AppendLine("        __lenPos = writer.BaseStream.Position;");
        sb.AppendLine("        writer.Write(0);");
        sb.AppendLine("        __start = writer.BaseStream.Position;");
        sb.AppendLine();

        EmitWritePayload(sb, f, info);

        sb.AppendLine();
        sb.AppendLine("        __end = writer.BaseStream.Position;");
        sb.AppendLine("        __payloadLen = checked((int)(__end - __start));");
        sb.AppendLine("        writer.BaseStream.Position = __lenPos;");
        sb.AppendLine("        writer.Write(__payloadLen);");
        sb.AppendLine("        writer.BaseStream.Position = __end;");
        sb.AppendLine();
    }

    private static void EmitWritePayload(StringBuilder sb, FieldInfo f, FieldTypeInfo info)
    {
        string valueExpr = $"this.{f.Name}";

        switch (info.Kind)
        {
            case FieldKind.Primitive:
            case FieldKind.Special:
                sb.AppendLine($"        writer.Write({valueExpr});");
                return;

            case FieldKind.SerializableEnum:
                sb.AppendLine($"        writer.WriteSerializableEnum({valueExpr});");
                return;

            case FieldKind.NullableValue:
            {
                sb.AppendLine($"        if ({valueExpr}.HasValue)");
                sb.AppendLine("        {");
                sb.AppendLine("            writer.Write(true);");
                EmitWriteInnerNullableValue(sb, info.Inner!, $"{valueExpr}.Value");
                sb.AppendLine("        }");
                sb.AppendLine("        else");
                sb.AppendLine("        {");
                sb.AppendLine("            writer.Write(false);");
                sb.AppendLine("        }");
                return;
            }

            case FieldKind.Object:
                // No nested length: just write nested message bytes into this field payload
                sb.AppendLine($"        {valueExpr}.Serialize(writer);");
                return;

            case FieldKind.NullableObject:
                sb.AppendLine($"        if ({valueExpr} is null)");
                sb.AppendLine("        {");
                sb.AppendLine("            writer.Write(false);");
                sb.AppendLine("        }");
                sb.AppendLine("        else");
                sb.AppendLine("        {");
                sb.AppendLine("            writer.Write(true);");
                sb.AppendLine($"            {valueExpr}.Serialize(writer);");
                sb.AppendLine("        }");
                return;

            case FieldKind.List:
            case FieldKind.NullableList:
            {
                string      listExpr       = valueExpr;
                ITypeSymbol elemType       = info.ElementType!;
                string      elemFq         = elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                bool        isNullableList = info.Kind == FieldKind.NullableList;

                if (isNullableList)
                {
                    sb.AppendLine($"        if ({listExpr} is null)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            writer.Write(false);");
                    sb.AppendLine("        }");
                    sb.AppendLine("        else");
                    sb.AppendLine("        {");
                    sb.AppendLine("            writer.Write(true);");
                }

                // primitive/special lists via your extensions
                if (TryMapPrimitive(elemType, out _))
                {
                    sb.AppendLine($"            writer.Write({listExpr});");
                }
                else if (elemType.ToDisplayString() == "System.Guid" ||
                         elemType.ToDisplayString() == "System.DateTime" ||
                         elemType.ToDisplayString() == "System.TimeSpan" ||
                         elemType.ToDisplayString() == "System.Version" ||
                         elemType.ToDisplayString() == "System.Net.HttpStatusCode")
                {
                    sb.AppendLine($"            writer.Write({listExpr});");
                }
                else if (elemType.TypeKind == TypeKind.Enum && HasAttribute(elemType, _SERIALIZABLE_ENUM_ATTR))
                {
                    sb.AppendLine($"            writer.WriteSerializableEnumList<{elemFq}>({listExpr});");
                }
                else if (IsProtoMsgBaseDerived(elemType))
                {
                    // List of objects (supports null elements too): count + for each: bool has + int len + bytes
                    sb.AppendLine($"            writer.Write({listExpr}.Count);");
                    sb.AppendLine($"            for (int __i = 0; __i < {listExpr}.Count; __i++)");
                    sb.AppendLine("            {");
                    sb.AppendLine($"                var __item = {listExpr}[__i];");
                    sb.AppendLine("                if (__item is null)");
                    sb.AppendLine("                {");
                    sb.AppendLine("                    writer.Write(false);");
                    sb.AppendLine("                }");
                    sb.AppendLine("                else");
                    sb.AppendLine("                {");
                    sb.AppendLine("                    writer.Write(true);");

                    // per-item length patching inside the list payload (no allocations)
                    sb.AppendLine("                    long __itemLenPos = writer.BaseStream.Position;");
                    sb.AppendLine("                    writer.Write(0);");
                    sb.AppendLine("                    long __itemStart = writer.BaseStream.Position;");
                    sb.AppendLine("                    __item.Serialize(writer);");
                    sb.AppendLine("                    long __itemEnd = writer.BaseStream.Position;");
                    sb.AppendLine("                    int __itemLen = checked((int)(__itemEnd - __itemStart));");
                    sb.AppendLine("                    writer.BaseStream.Position = __itemLenPos;");
                    sb.AppendLine("                    writer.Write(__itemLen);");
                    sb.AppendLine("                    writer.BaseStream.Position = __itemEnd;");
                    sb.AppendLine("                }");
                    sb.AppendLine("            }");
                }

                if (isNullableList)
                {
                    sb.AppendLine("        }");
                }

                return;
            }
            case FieldKind.Array:
            case FieldKind.NullableArray:
            {
                string      arrayExpr       = valueExpr;
                ITypeSymbol elemType        = info.ElementType!;
                string      elemFq          = elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                bool        isNullableArray = info.Kind == FieldKind.NullableArray;

                if (isNullableArray)
                {
                    sb.AppendLine($"        if ({arrayExpr} is null)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            writer.Write(false);");
                    sb.AppendLine("        }");
                    sb.AppendLine("        else");
                    sb.AppendLine("        {");
                    sb.AppendLine("            writer.Write(true);");
                }

                // primitive/special arrays via your extensions
                if (TryMapPrimitive(elemType, out _))
                {
                    sb.AppendLine($"            writer.WriteArray({arrayExpr});");
                }
                else if (elemType.ToDisplayString() == "System.Guid" ||
                         elemType.ToDisplayString() == "System.DateTime" ||
                         elemType.ToDisplayString() == "System.TimeSpan" ||
                         elemType.ToDisplayString() == "System.Version" ||
                         elemType.ToDisplayString() == "System.Net.HttpStatusCode")
                {
                    sb.AppendLine($"            writer.Write({arrayExpr});");
                }
                else if (elemType.TypeKind == TypeKind.Enum && HasAttribute(elemType, _SERIALIZABLE_ENUM_ATTR))
                {
                    sb.AppendLine($"            writer.WriteSerializableEnumArray<{elemFq}>({arrayExpr});");
                }
                else if (IsProtoMsgBaseDerived(elemType))
                {
                    // Array of objects (supports null elements too): count + for each: bool has + int len + bytes
                    sb.AppendLine($"            writer.Write({arrayExpr}.Length);");
                    sb.AppendLine($"            for (int __i = 0; __i < {arrayExpr}.Length; __i++)");
                    sb.AppendLine("            {");
                    sb.AppendLine($"                var __item = {arrayExpr}[__i];");
                    sb.AppendLine("                if (__item is null)");
                    sb.AppendLine("                {");
                    sb.AppendLine("                    writer.Write(false);");
                    sb.AppendLine("                }");
                    sb.AppendLine("                else");
                    sb.AppendLine("                {");
                    sb.AppendLine("                    writer.Write(true);");

                    // per-item length patching inside the array payload (no allocations)
                    sb.AppendLine("                    long __itemLenPos = writer.BaseStream.Position;");
                    sb.AppendLine("                    writer.Write(0);");
                    sb.AppendLine("                    long __itemStart = writer.BaseStream.Position;");
                    sb.AppendLine("                    __item.Serialize(writer);");
                    sb.AppendLine("                    long __itemEnd = writer.BaseStream.Position;");
                    sb.AppendLine("                    int __itemLen = checked((int)(__itemEnd - __itemStart));");
                    sb.AppendLine("                    writer.BaseStream.Position = __itemLenPos;");
                    sb.AppendLine("                    writer.Write(__itemLen);");
                    sb.AppendLine("                    writer.BaseStream.Position = __itemEnd;");
                    sb.AppendLine("                }");
                    sb.AppendLine("            }");
                }

                if (isNullableArray)
                {
                    sb.AppendLine("        }");
                }

                return;
            }
        }
    }

    private static void EmitWriteInnerNullableValue(StringBuilder sb, FieldTypeInfo inner, string valueExpr)
    {
        // inner.Kind is constrained in TryClassifyFieldType
        switch (inner.Kind)
        {
            case FieldKind.Primitive:
            case FieldKind.Special:
                sb.AppendLine($"            writer.Write({valueExpr});");
                return;

            case FieldKind.SerializableEnum:
                sb.AppendLine($"            writer.WriteSerializableEnum({valueExpr});");
                return;
        }
    }

    private static void EmitDeserializeField(StringBuilder sb, FieldInfo f, FieldTypeInfo info)
    {
        switch (info.Kind)
        {
            case FieldKind.Primitive:
                if (TryMapPrimitive(f.Type, out string read))
                    sb.AppendLine($"                    this.{f.Name} = reader.{read}();");
                return;

            case FieldKind.Special:
                sb.AppendLine($"                    this.{f.Name} = reader.Read{f.Type.ToDisplayString().Split('.').Last()}();");
                return;

            case FieldKind.SerializableEnum:
            {
                string fqEnum = f.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                sb.AppendLine($"                    this.{f.Name} = reader.ReadSerializableEnum<{fqEnum}>();");
                return;
            }

            case FieldKind.NullableValue:
            {
                ITypeSymbol innerType = ((INamedTypeSymbol)f.Type).TypeArguments[0];
                sb.AppendLine("                    {");
                sb.AppendLine("                        bool __has = reader.ReadBoolean();");
                sb.AppendLine("                        if (!__has)");
                sb.AppendLine($"                            this.{f.Name} = null;");
                sb.AppendLine("                        else");
                sb.AppendLine("                        {");
                sb.AppendLine($"                            this.{f.Name} = {EmitReadValueExpr(innerType)};");
                sb.AppendLine("                        }");
                sb.AppendLine("                    }");
                return;
            }

            case FieldKind.Object:
            {
                // Read remaining bytes in this field as the nested message blob
                string fqObj = f.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                sb.AppendLine("                    {");
                sb.AppendLine("                        int __remaining = checked((int)(end - reader.BaseStream.Position));");
                sb.AppendLine("                        byte[] __payload = reader.ReadBytes(__remaining);");
                sb.AppendLine($"                        var __obj = new {fqObj}();");
                sb.AppendLine("                        using var __ms = new MemoryStream(__payload);");
                sb.AppendLine("                        using var __br = new BinaryReader(__ms, Encoding.UTF8, false);");
                sb.AppendLine("                        __obj.Deserialize(__br);");
                sb.AppendLine($"                        this.{f.Name} = __obj;");
                sb.AppendLine("                    }");
                return;
            }

            case FieldKind.NullableObject:
            {
                string fqObj = f.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                sb.AppendLine("                    {");
                sb.AppendLine("                        bool __has = reader.ReadBoolean();");
                sb.AppendLine("                        if (!__has)");
                sb.AppendLine($"                            this.{f.Name} = null;");
                sb.AppendLine("                        else");
                sb.AppendLine("                        {");
                sb.AppendLine("                            int __remaining = checked((int)(end - reader.BaseStream.Position));");
                sb.AppendLine("                            byte[] __payload = reader.ReadBytes(__remaining);");
                sb.AppendLine($"                            var __obj = new {fqObj}();");
                sb.AppendLine("                            using var __ms = new MemoryStream(__payload);");
                sb.AppendLine("                            using var __br = new BinaryReader(__ms, Encoding.UTF8, false);");
                sb.AppendLine("                            __obj.Deserialize(__br);");
                sb.AppendLine($"                            this.{f.Name} = __obj;");
                sb.AppendLine("                        }");
                sb.AppendLine("                    }");
                return;
            }

            case FieldKind.List:
            case FieldKind.NullableList:
            {
                bool        isNullable = info.Kind == FieldKind.NullableList;
                ITypeSymbol elemType   = info.ElementType!;
                string      elemFq     = elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (isNullable)
                {
                    sb.AppendLine("                    {");
                    sb.AppendLine("                        bool __hasList = reader.ReadBoolean();");
                    sb.AppendLine("                        if (!__hasList)");
                    sb.AppendLine($"                            this.{f.Name} = null;");
                    sb.AppendLine("                        else");
                    sb.AppendLine("                        {");
                }

                string indent = isNullable ? "                            " : "                    ";

                if (TryMapPrimitive(elemType, out _))
                {
                    sb.AppendLine($"{indent}this.{f.Name} = {GetListReadCall(elemType)};");
                }
                else if (_SpecialTypes.Contains(elemType.ToDisplayString()))
                {
                    sb.AppendLine($"{indent}this.{f.Name} = reader.Read{elemType.ToDisplayString().Split('.').Last()}List();");
                }
                else if (elemType.TypeKind == TypeKind.Enum && HasAttribute(elemType, _SERIALIZABLE_ENUM_ATTR))
                {
                    sb.AppendLine($"{indent}this.{f.Name} = reader.ReadSerializableEnumList<{elemFq}>();");
                }
                else if (IsProtoMsgBaseDerived(elemType))
                {
                    // count + for each: bool has + int len + bytes
                    sb.AppendLine($"{indent}{{");
                    sb.AppendLine($"{indent}    int __count = reader.ReadInt32();");
                    sb.AppendLine($"{indent}    var __list = new List<{elemFq}>(__count);");
                    sb.AppendLine($"{indent}    for (int __i = 0; __i < __count; __i++)");
                    sb.AppendLine($"{indent}    {{");
                    sb.AppendLine($"{indent}        bool __has = reader.ReadBoolean();");
                    sb.AppendLine($"{indent}        if (!__has)");
                    sb.AppendLine($"{indent}        {{");
                    sb.AppendLine($"{indent}            __list.Add(null!);");
                    sb.AppendLine($"{indent}        }}");
                    sb.AppendLine($"{indent}        else");
                    sb.AppendLine($"{indent}        {{");
                    sb.AppendLine($"{indent}            int __len = reader.ReadInt32();");
                    sb.AppendLine($"{indent}            byte[] __bytes = reader.ReadBytes(__len);");
                    sb.AppendLine($"{indent}            var __obj = new {elemFq}();");
                    sb.AppendLine($"{indent}            using var __ms = new MemoryStream(__bytes);");
                    sb.AppendLine($"{indent}            using var __br = new BinaryReader(__ms, Encoding.UTF8, false);");
                    sb.AppendLine($"{indent}            __obj.Deserialize(__br);");
                    sb.AppendLine($"{indent}            __list.Add(__obj);");
                    sb.AppendLine($"{indent}        }}");
                    sb.AppendLine($"{indent}    }}");
                    sb.AppendLine($"{indent}    this.{f.Name} = __list;");
                    sb.AppendLine($"{indent}}}");
                }

                if (isNullable)
                {
                    sb.AppendLine("                        }");
                    sb.AppendLine("                    }");
                }

                return;
            }
            case FieldKind.Array:
            case FieldKind.NullableArray:
            {
                bool        isNullable = info.Kind == FieldKind.NullableArray;
                ITypeSymbol elemType   = info.ElementType!;
                string      elemFq     = elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (isNullable)
                {
                    sb.AppendLine("                    {");
                    sb.AppendLine("                        bool __hasArray = reader.ReadBoolean();");
                    sb.AppendLine("                        if (!__hasArray)");
                    sb.AppendLine($"                            this.{f.Name} = null;");
                    sb.AppendLine("                        else");
                    sb.AppendLine("                        {");
                }

                string indent = isNullable ? "                            " : "                    ";

                if (TryMapPrimitive(elemType, out _))
                {
                    sb.AppendLine($"{indent}this.{f.Name} = {GetArrayReadCall(elemType)};");
                }
                else if (_SpecialTypes.Contains(elemType.ToDisplayString()))
                {
                    sb.AppendLine($"{indent}this.{f.Name} = reader.Read{elemType.ToDisplayString().Split('.').Last()}Array();");
                }
                else if (elemType.TypeKind == TypeKind.Enum && HasAttribute(elemType, _SERIALIZABLE_ENUM_ATTR))
                {
                    sb.AppendLine($"{indent}this.{f.Name} = reader.ReadSerializableEnumArray<{elemFq}>();");
                }
                else if (IsProtoMsgBaseDerived(elemType))
                {
                    // count + for each: bool has + int len + bytes
                    sb.AppendLine($"{indent}{{");
                    sb.AppendLine($"{indent}    int __count = reader.ReadInt32();");
                    sb.AppendLine($"{indent}    var __array = new {elemFq}[__count];");
                    sb.AppendLine($"{indent}    for (int __i = 0; __i < __count; __i++)");
                    sb.AppendLine($"{indent}    {{");
                    sb.AppendLine($"{indent}        bool __has = reader.ReadBoolean();");
                    sb.AppendLine($"{indent}        if (!__has)");
                    sb.AppendLine($"{indent}        {{");
                    sb.AppendLine($"{indent}            __array[__i] = null!;");
                    sb.AppendLine($"{indent}        }}");
                    sb.AppendLine($"{indent}        else");
                    sb.AppendLine($"{indent}        {{");
                    sb.AppendLine($"{indent}            int __len = reader.ReadInt32();");
                    sb.AppendLine($"{indent}            byte[] __bytes = reader.ReadBytes(__len);");
                    sb.AppendLine($"{indent}            var __obj = new {elemFq}();");
                    sb.AppendLine($"{indent}            using var __ms = new MemoryStream(__bytes);");
                    sb.AppendLine($"{indent}            using var __br = new BinaryReader(__ms, Encoding.UTF8, false);");
                    sb.AppendLine($"{indent}            __obj.Deserialize(__br);");
                    sb.AppendLine($"{indent}            __array[__i] = __obj;");
                    sb.AppendLine($"{indent}        }}");
                    sb.AppendLine($"{indent}    }}");
                    sb.AppendLine($"{indent}    this.{f.Name} = __array;");
                    sb.AppendLine($"{indent}}}");
                }

                if (isNullable)
                {
                    sb.AppendLine("                        }");
                    sb.AppendLine("                    }");
                }

                return;
            }
        }
    }

    private static string EmitReadValueExpr(ITypeSymbol type)
    {
        if (TryMapPrimitive(type, out string read))
            return $"reader.{read}()";

        string full = type.ToDisplayString();

        if (_SpecialTypes.Contains(full)) return $"reader.Read{full.Split('.').Last()}()";

        if (type.TypeKind == TypeKind.Enum && HasAttribute(type, _SERIALIZABLE_ENUM_ATTR))
        {
            string fq = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return $"reader.ReadSerializableEnum<{fq}>()";
        }

        return "default";
    }

    private static string GetListReadCall(ITypeSymbol elemType)
    {
        return elemType.SpecialType switch
        {
            SpecialType.System_Byte    => "reader.ReadByteList()",
            SpecialType.System_SByte   => "reader.ReadSByteList()",
            SpecialType.System_Boolean => "reader.ReadBoolList()",
            SpecialType.System_Int16   => "reader.ReadShortList()",
            SpecialType.System_UInt16  => "reader.ReadUShortList()",
            SpecialType.System_Int32   => "reader.ReadIntList()",
            SpecialType.System_UInt32  => "reader.ReadUIntList()",
            SpecialType.System_Int64   => "reader.ReadLongList()",
            SpecialType.System_UInt64  => "reader.ReadULongList()",
            SpecialType.System_Single  => "reader.ReadFloatList()",
            SpecialType.System_Double  => "reader.ReadDoubleList()",
            SpecialType.System_Char    => "reader.ReadCharList()",
            SpecialType.System_String  => "reader.ReadStringList()",
            _                          => "throw new InvalidOperationException(\"Unsupported list element\")"
        };
    }

    private static string GetArrayReadCall(ITypeSymbol elemType)
    {
        return elemType.SpecialType switch
        {
            SpecialType.System_Byte    => "reader.ReadByteArray()",
            SpecialType.System_SByte   => "reader.ReadSByteArray()",
            SpecialType.System_Boolean => "reader.ReadBoolArray()",
            SpecialType.System_Int16   => "reader.ReadShortArray()",
            SpecialType.System_UInt16  => "reader.ReadUShortArray()",
            SpecialType.System_Int32   => "reader.ReadIntArray()",
            SpecialType.System_UInt32  => "reader.ReadUIntArray()",
            SpecialType.System_Int64   => "reader.ReadLongArray()",
            SpecialType.System_UInt64  => "reader.ReadULongArray()",
            SpecialType.System_Single  => "reader.ReadFloatArray()",
            SpecialType.System_Double  => "reader.ReadDoubleArray()",
            SpecialType.System_Char    => "reader.ReadCharArray()",
            SpecialType.System_String  => "reader.ReadStringArray()",
            _                          => "throw new InvalidOperationException(\"Unsupported array element\")"
        };
    }
}