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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DeusaldSharp.Analyzers;

// The incremental pipeline only carries these value-equatable models (no symbols, no syntax, no compilation),
// so the output stage is skipped whenever a re-run of the transform produces an equal model.

internal enum ProtoFieldKind
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

internal enum ProtoValueKind
{
    Primitive,
    Special,
    SerializableEnum,
    Object,
}

/// <param name="Id">Field id.</param>
/// <param name="Name">Member name, escaped with '@' when it is a keyword.</param>
/// <param name="IsInherited">True when the field is declared on a base message (accessed through 'base.').</param>
/// <param name="Display">Human readable "Type Name" used in generated comments.</param>
/// <param name="Kind">Shape of the field.</param>
/// <param name="ValueKind">Kind of the scalar value, the Nullable inner value or the collection element.</param>
/// <param name="ValueType">Fully qualified type of the scalar value, the Nullable inner value or the collection element.</param>
/// <param name="ReadMethod">Scalar read method (e.g. ReadInt32, ReadGuid). Empty when not applicable.</param>
/// <param name="CollectionReadMethod">Collection read method (e.g. ReadIntList, ReadGuidArray). Empty when not applicable.</param>
/// <param name="IsReferenceValue">True for scalar reference types (string, Version): the field is omitted when null.</param>
internal sealed record ProtoFieldModel(
    int            Id,
    string         Name,
    bool           IsInherited,
    string         Display,
    ProtoFieldKind Kind,
    ProtoValueKind ValueKind,
    string         ValueType,
    string         ReadMethod,
    string         CollectionReadMethod,
    bool           IsReferenceValue)
{
    public string Access => (IsInherited ? "base." : "this.") + Name;
}

internal sealed record LocationInfo(string FilePath, TextSpan Span, LinePositionSpan LineSpan)
{
    public Location ToLocation() => Location.Create(FilePath, Span, LineSpan);

    public static LocationInfo? From(Location? location)
    {
        if (location is null || location.SourceTree is null) return null;
        return new LocationInfo(location.SourceTree.FilePath, location.SourceSpan, location.GetLineSpan().Span);
    }
}

internal sealed record DiagnosticInfo(string DescriptorId, LocationInfo? Location, EquatableArray<string> Arguments);

/// <param name="HintName">Unique, file-name safe hint name.</param>
/// <param name="Namespace">Containing namespace or null for the global namespace.</param>
/// <param name="ContainingDeclarations">Declarations of the containing types, outermost first (e.g. "partial class Outer&lt;T&gt;").</param>
/// <param name="Declaration">Declaration of the message itself (e.g. "partial class Msg").</param>
/// <param name="FullyQualifiedName">Fully qualified name of the message used to create a fresh instance.</param>
/// <param name="CanReset">True when a fresh instance can be created to reset fields.</param>
/// <param name="Generate">False when only diagnostics should be reported.</param>
/// <param name="Fields">All fields of the inheritance chain sorted by id.</param>
/// <param name="Diagnostics">Diagnostics to report.</param>
internal sealed record ProtoMessageModel(
    string                         HintName,
    string?                        Namespace,
    EquatableArray<string>         ContainingDeclarations,
    string                         Declaration,
    string                         FullyQualifiedName,
    bool                           CanReset,
    bool                           Generate,
    EquatableArray<ProtoFieldModel> Fields,
    EquatableArray<DiagnosticInfo>  Diagnostics);

/// <summary> Immutable array with value equality, usable inside incremental generator models. </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly T[]? _Items;

    public EquatableArray(IEnumerable<T> items) => _Items = items.ToArray();

    public int Count => _Items?.Length ?? 0;

    public T this[int index] => _Items![index];

    public bool Equals(EquatableArray<T> other)
        => (_Items ?? []).SequenceEqual(other._Items ?? []);

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        if (_Items is null) return 0;

        int hash = 17;
        foreach (T item in _Items) hash = hash * 31 + (item?.GetHashCode() ?? 0);
        return hash;
    }

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)(_Items ?? [])).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static EquatableArray<T> Empty => new([]);
}
