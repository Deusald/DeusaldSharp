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
using Microsoft.CodeAnalysis;

namespace DeusaldSharp.Analyzers;

[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking")]
internal static class ProtoDiagnostics
{
    private const string _CATEGORY = "DeusaldSharp.Proto";

    public static readonly DiagnosticDescriptor MustBePartial = _Error(
        "DSHP001",
        "Proto message must be partial",
        "Type '{0}' derives from ProtoMsgBase but is not partial. Mark it 'partial' so code can be generated.");

    public static readonly DiagnosticDescriptor InvalidId = _Error(
        "DSHP002",
        "Invalid ProtoField id",
        "Field '{0}' has an invalid [ProtoField] id. Id must be a non-zero ushort.");

    public static readonly DiagnosticDescriptor DuplicateId = _Error(
        "DSHP003",
        "Duplicate ProtoField id",
        "Type '{0}' contains duplicate [ProtoField({1})]. Field ids must be unique per message.");

    public static readonly DiagnosticDescriptor UnsupportedType = _Error(
        "DSHP004",
        "Unsupported field type",
        "Field '{0}' has unsupported type '{1}'.");

    public static readonly DiagnosticDescriptor GenericFieldNeedsNew = _Error(
        "DSHP005",
        "Generic Proto field type must have new() constraint",
        "Field '{0}' uses generic type parameter '{1}' which is deserialized via 'new {1}()'. Add 'where {1} : new()' to the containing type.");

    public static readonly DiagnosticDescriptor GenericListNeedsNew = _Error(
        "DSHP006",
        "Generic List Proto field type must have new() constraint",
        "Field '{0}' uses generic type parameter '{1}' which is deserialized via 'new {1}()'. Add 'where {1} : new()' to the containing type.");

    public static readonly DiagnosticDescriptor GenericArrayNeedsNew = _Error(
        "DSHP007",
        "Generic Array Proto field type must have new() constraint",
        "Field '{0}' uses generic type parameter '{1}' which is deserialized via 'new {1}()'. Add 'where {1} : new()' to the containing type.");

    public static readonly DiagnosticDescriptor ReadOnlyField = _Error(
        "DSHP008",
        "ProtoField must be writable",
        "Field '{0}' is readonly (or a property without a non-init setter) and cannot be deserialized. Remove 'readonly' or the [ProtoField] attribute.");

    public static readonly DiagnosticDescriptor StaticField = _Error(
        "DSHP009",
        "ProtoField must be an instance field",
        "Field '{0}' is static or const. [ProtoField] is only supported on instance fields.");

    public static readonly DiagnosticDescriptor DuplicateInheritedId = _Error(
        "DSHP010",
        "Duplicate ProtoField id in inheritance chain",
        "Field '{0}' uses [ProtoField({1})] which is already used by field '{2}' declared on base message '{3}'. Field ids must be unique across the whole inheritance chain.");

    public static readonly DiagnosticDescriptor InaccessibleInheritedField = _Error(
        "DSHP011",
        "Inherited ProtoField is not accessible",
        "Field '{0}' declared on base message '{1}' is not accessible from derived message '{2}'. Make it protected, internal or public so the generated code of '{2}' can serialize it.");

    public static readonly DiagnosticDescriptor ContainingTypeMustBePartial = _Error(
        "DSHP012",
        "Containing type of a proto message must be partial",
        "Type '{0}' contains proto message '{1}' but is not partial. Mark it 'partial' so code can be generated.");

    private static readonly Dictionary<string, DiagnosticDescriptor> _ById = new[]
    {
        MustBePartial, InvalidId, DuplicateId, UnsupportedType, GenericFieldNeedsNew, GenericListNeedsNew, GenericArrayNeedsNew,
        ReadOnlyField, StaticField, DuplicateInheritedId, InaccessibleInheritedField, ContainingTypeMustBePartial
    }.ToDictionary(d => d.Id);

    public static DiagnosticInfo Create(DiagnosticDescriptor descriptor, Location? location, params object[] arguments)
        => new(descriptor.Id, LocationInfo.From(location), new EquatableArray<string>(arguments.Select(a => a.ToString())));

    public static Diagnostic ToDiagnostic(DiagnosticInfo info)
        => Diagnostic.Create(
            _ById[info.DescriptorId],
            info.Location?.ToLocation(),
            info.Arguments.Cast<object>().ToArray());

    private static DiagnosticDescriptor _Error(string id, string title, string message)
        => new(id, title, message, _CATEGORY, DiagnosticSeverity.Error, isEnabledByDefault: true);
}
