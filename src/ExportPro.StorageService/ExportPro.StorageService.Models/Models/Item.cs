﻿using ExportPro.Common.Models.MongoDB;
using ExportPro.StorageService.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.StorageService.Models.Models;

public sealed class Item : AuditModel, IModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId? CustomerId { get; set; }

    public Status? Status { get; set; }
    public ObjectId CurrencyId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ObjectId Id { get; set; }
}
