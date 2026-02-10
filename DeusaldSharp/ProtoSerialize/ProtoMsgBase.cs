using System;
using System.IO;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    /// <summary>
    /// Base type for all proto messages.
    /// Generated code must override Serialize(BinaryWriter) and Deserialize(BinaryReader).
    /// </summary>
    [PublicAPI]
    public abstract class ProtoMsgBase
    {
        // These are the "must override" entry points.
        public virtual void Serialize(BinaryWriter w)
            => throw new NotImplementedException("Serialize(BinaryWriter) was not generated for this message type.");

        public virtual void Deserialize(BinaryReader r)
            => throw new NotImplementedException("Deserialize(BinaryReader) was not generated for this message type.");
        
        public byte[] Serialize()
        {
            return Helpers.SerializeByBinaryWriter(Serialize);
        }
        
        public void Deserialize(byte[] data)
        {
            Helpers.DeserializeFromBinaryReader(data, Deserialize);
        }
        
        public static T Deserialize<T>(byte[] data)
            where T : ProtoMsgBase, new()
        {
            T msg = new T();
            Helpers.DeserializeFromBinaryReader(data, r => msg.Deserialize(r));
            return msg;
        }
    }
}