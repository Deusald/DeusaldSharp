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
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

// ReSharper disable StaticMemberInGenericType

namespace DeusaldSharp
{
    public static class SerializableEnumCache<TEnum> where TEnum : unmanaged, Enum
    {
        private static readonly bool                 _HasAttribute;
        private static readonly SerializableEnumType _AttrWireType;
        private static readonly int                  _EnumSize = Unsafe.SizeOf<TEnum>(); // 1/2/4/8 expected

        static SerializableEnumCache()
        {
            SerializableEnumAttribute? attr = typeof(TEnum).GetCustomAttribute<SerializableEnumAttribute>(inherit: false);
            if (attr == null)
            {
                _HasAttribute = false;
                _AttrWireType = default;
            }
            else
            {
                _HasAttribute = true;
                _AttrWireType = attr.EnumType;
            }
        }

        private static SerializableEnumType ResolveWireTypeOrThrow()
        {
            if (_HasAttribute) return _AttrWireType;
            throw new InvalidOperationException($"{typeof(TEnum).FullName} must be annotated with [SerializableEnum].");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ReadRawBits(TEnum value)
        {
            // Reinterpret underlying bytes into an unsigned bucket (no boxing).
            return _EnumSize switch
            {
                1 => Unsafe.As<TEnum, byte>(ref value),
                2 => Unsafe.As<TEnum, ushort>(ref value),
                4 => Unsafe.As<TEnum, uint>(ref value),
                8 => Unsafe.As<TEnum, ulong>(ref value),
                _ => throw new NotSupportedException($"Unexpected enum size {_EnumSize} for {typeof(TEnum)}.")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSignedWireType(SerializableEnumType wireType)
        {
            return wireType == SerializableEnumType.SByte
                || wireType == SerializableEnumType.Short
                || wireType == SerializableEnumType.Int
                || wireType == SerializableEnumType.Long;
        }

        /// <summary>
        /// Interprets the enum raw bits as a signed value using the enum storage size.
        /// This enables correct serialization of negative enum values without boxing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long SignExtendEnumBits(ulong rawBits)
        {
            return _EnumSize switch
            {
                1 => (sbyte)(byte)rawBits,
                2 => (short)(ushort)rawBits,
                4 => (int)(uint)rawBits,
                8 => (long)rawBits,
                _ => throw new NotSupportedException($"Unexpected enum size {_EnumSize} for {typeof(TEnum)}.")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteWithWireType(BinaryWriter writer, SerializableEnumType wireType, ulong u)
        {
            // If the chosen wire type is signed, we must sign-extend based on enum storage size.
            // Example: (sbyte)-1 has raw bits 0xFF; casting ulong->long would produce 255, which overflows sbyte.
            long s = IsSignedWireType(wireType) ? SignExtendEnumBits(u) : unchecked((long)u);

            try
            {
                switch (wireType)
                {
                    case SerializableEnumType.Byte:
                        writer.Write(checked((byte)u));
                        return;
                    case SerializableEnumType.SByte:
                        writer.Write(checked((sbyte)s));
                        return;
                    case SerializableEnumType.UShort:
                        writer.Write(checked((ushort)u));
                        return;
                    case SerializableEnumType.Short:
                        writer.Write(checked((short)s));
                        return;
                    case SerializableEnumType.UInt:
                        writer.Write(checked((uint)u));
                        return;
                    case SerializableEnumType.Int:
                        writer.Write(checked((int)s));
                        return;
                    case SerializableEnumType.ULong:
                        writer.Write(u);
                        return;
                    case SerializableEnumType.Long:
                        writer.Write(s);
                        return;
                    default:
                        throw new NotSupportedException($"Unsupported wire type {wireType}.");
                }
            }
            catch (OverflowException ex)
            {
                throw new InvalidOperationException($"Enum value does not fit {wireType}.", ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ReadWithWireType(BinaryReader reader, SerializableEnumType wireType)
        {
            return wireType switch
            {
                SerializableEnumType.Byte   => reader.ReadByte(),
                SerializableEnumType.SByte  => unchecked((ulong)reader.ReadSByte()),
                SerializableEnumType.UShort => reader.ReadUInt16(),
                SerializableEnumType.Short  => unchecked((ulong)reader.ReadInt16()),
                SerializableEnumType.UInt   => reader.ReadUInt32(),
                SerializableEnumType.Int    => unchecked((ulong)reader.ReadInt32()),
                SerializableEnumType.ULong  => reader.ReadUInt64(),
                SerializableEnumType.Long   => unchecked((ulong)reader.ReadInt64()),
                _                           => throw new NotSupportedException($"Unsupported wire type {wireType}.")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TEnum BitsToEnum(ulong u, SerializableEnumType wireType)
        {
            int bits = _EnumSize * 8;
            if (bits < 64)
            {
                ulong mask      = (1UL << bits) - 1UL;
                ulong truncated = u & mask;

                if (IsSignedWireType(wireType))
                {
                    // Allow proper sign-extension for signed wire values.
                    bool  signBitSet = (truncated & (1UL << (bits - 1))) != 0UL;
                    ulong upper      = u & ~mask;

                    if (signBitSet)
                    {
                        if (upper != ~mask)
                            throw new InvalidOperationException($"Wire value does not fit into enum storage ({typeof(TEnum).Name}, {_EnumSize} bytes) while reading {wireType}.");
                    }
                    else
                    {
                        if (upper != 0UL)
                            throw new InvalidOperationException($"Wire value does not fit into enum storage ({typeof(TEnum).Name}, {_EnumSize} bytes) while reading {wireType}.");
                    }

                    u = truncated;
                }
                else
                {
                    if ((u & ~mask) != 0UL)
                        throw new InvalidOperationException($"Wire value does not fit into enum storage ({typeof(TEnum).Name}, {_EnumSize} bytes) while reading {wireType}.");
                }
            }

            TEnum result = default;
            switch (_EnumSize)
            {
                case 1:  Unsafe.As<TEnum, byte>(ref result)   = (byte)u; break;
                case 2:  Unsafe.As<TEnum, ushort>(ref result) = (ushort)u; break;
                case 4:  Unsafe.As<TEnum, uint>(ref result)   = (uint)u; break;
                case 8:  Unsafe.As<TEnum, ulong>(ref result)  = u; break;
                default: throw new NotSupportedException($"Unexpected enum size {_EnumSize} for {typeof(TEnum)}.");
            }

            return result;
        }

        public static void Write(BinaryWriter writer, TEnum value)
        {
            SerializableEnumType wireType = ResolveWireTypeOrThrow();
            ulong                u        = ReadRawBits(value);
            WriteWithWireType(writer, wireType, u);
        }

        public static TEnum Read(BinaryReader reader)
        {
            SerializableEnumType wireType = ResolveWireTypeOrThrow();
            ulong                u        = ReadWithWireType(reader, wireType);
            return BitsToEnum(u, wireType);
        }
    }
}