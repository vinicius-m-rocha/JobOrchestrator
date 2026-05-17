using System.Runtime.InteropServices;
using System.Security.Cryptography;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Enums;
using JobOrchestrator.Domain.ValuesObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace JobOrchestrator.Infrastructure.Persistence;

public static class MongoDbMapping
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Job>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(j => j.Id);

            cm.MapProperty(j => j.Priority).SetSerializer(new EnumSerializer<JobPriority>(BsonType.String));
            cm.MapProperty(j => j.Status).SetSerializer(new EnumSerializer<JobStatus>(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<ExecutionAttempt>(cm =>
        {
            cm.AutoMap();
        });
    }
}