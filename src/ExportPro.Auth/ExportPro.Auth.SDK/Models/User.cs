﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using ExportPro.Common.Models.MongoDB;

namespace ExportPro.Auth.SDK.Models;

public class User : IModel
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public List<UserRole> Roles { get; set; } = [];
    public int TokenVersion { get; set; } = 0;
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}

