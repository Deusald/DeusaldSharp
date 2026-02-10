using System;

namespace DeusaldSharp
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ProtoFieldAttribute : Attribute
    {
        public ushort Id { get; }

        public ProtoFieldAttribute(ushort id) => Id = id;
    }
}