﻿using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ExportPro.Common.DataAccess.MongoDB.Services;

public class MongoDbConnectionFactory : IMongoDbConnectionFactory
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoDbConnectionFactory(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "ExportProDb";

        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
    }

    public IMongoDatabase GetDatabase() => _database;
    public IMongoClient GetClient() => _client;
}