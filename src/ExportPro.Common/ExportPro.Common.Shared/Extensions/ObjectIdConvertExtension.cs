using MongoDB.Bson;

namespace ExportPro.Common.Shared.Extensions;

public static class ObjectIdConvertExtension
{
    public static Guid ToGuid(this ObjectId objectId)
    {
        var objectIdBytes = objectId.ToByteArray();
        var guidBytes = new byte[16];
        Array.Copy(objectIdBytes, 0, guidBytes, 0, objectIdBytes.Length);
        return new Guid(guidBytes);
    }

    public static ObjectId ToObjectId(this Guid guid)
    {
        var guidBytes = guid.ToByteArray();
        var objectIdBytes = new byte[12];
        Array.Copy(guidBytes, 0, objectIdBytes, 0, objectIdBytes.Length);
        return new ObjectId(objectIdBytes);
    }
}
