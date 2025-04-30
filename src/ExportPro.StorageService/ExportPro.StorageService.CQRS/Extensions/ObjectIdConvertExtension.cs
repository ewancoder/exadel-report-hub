using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Extensions;

public static class ObjectIdConvertExtension
{
    public static Guid ToGuid(this ObjectId objectId)
    {
        byte[] objectIdBytes = objectId.ToByteArray();
        byte[] guidBytes = new byte[16];
        Array.Copy(objectIdBytes, 0, guidBytes, 0, objectIdBytes.Length);
        return new Guid(guidBytes);
    }

    public static Guid? ToGuidEvenNull(this ObjectId objectId)
    {
        byte[] objectIdBytes = objectId.ToByteArray();
        byte[] guidBytes = new byte[16];
        Array.Copy(objectIdBytes, 0, guidBytes, 0, objectIdBytes.Length);
        return new Guid(guidBytes);
    }

    public static ObjectId ToObjectId(this Guid guid)
    {
        byte[] guidBytes = guid.ToByteArray();
        byte[] objectIdBytes = new byte[12];
        Array.Copy(guidBytes, 0, objectIdBytes, 0, objectIdBytes.Length);
        return new ObjectId(objectIdBytes);
    }

    public static ObjectId? ToObjectIdEvenNull(this Guid guid)
    {
        byte[] guidBytes = guid.ToByteArray();
        byte[] objectIdBytes = new byte[12];
        Array.Copy(guidBytes, 0, objectIdBytes, 0, objectIdBytes.Length);
        return new ObjectId(objectIdBytes);
    }
}
