using GPGO_MultiPLCs.GP_PLCs;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GPGO_MultiPLCs.Helpers
{
    public class BsonDataLoc : SerializerBase<(DataType, int)>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, (DataType, int) value)
        {
            if (context == null)
            {
                return;
            }

            var (v1, v2) = value;
            context.Writer.WriteStartArray();
            context.Writer.WriteInt32((int)v1);
            context.Writer.WriteInt32(v2);
            context.Writer.WriteEndArray();
        }

        public override (DataType, int) Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context == null)
            {
                return (0, 0);
            }

            context.Reader.ReadStartArray();
            var v1 = context.Reader.ReadInt32();
            var v2 = context.Reader.ReadInt32();
            context.Reader.ReadEndArray();

            return ((DataType)v1, v2);
        }
    }

    public class BsonBitLoc : SerializerBase<(BitType, int)>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, (BitType, int) value)
        {
            if (context == null)
            {
                return;
            }

            var (v1, v2) = value;
            context.Writer.WriteStartArray();
            context.Writer.WriteInt32((int)v1);
            context.Writer.WriteInt32(v2);
            context.Writer.WriteEndArray();
        }

        public override (BitType, int) Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context == null)
            {
                return (0, 0);
            }

            context.Reader.ReadStartArray();
            var v1 = context.Reader.ReadInt32();
            var v2 = context.Reader.ReadInt32();
            context.Reader.ReadEndArray();

            return ((BitType)v1, v2);
        }
    }
}
