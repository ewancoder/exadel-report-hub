using MongoDB.Bson;


namespace ExportPro.Common.Models.MongoDB
{
    public interface IModel
    {
        ObjectId Id { get; set; }
    }
}
