using api.helperclasses;
using api.models.api;
using api.pieces;
using api.pieces.interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace api.models.db;

public static class DatabaseInit
{
    public static void RegisterModels()
    {
        var objectSerializer = new ObjectSerializer(type =>
            ObjectSerializer.DefaultAllowedTypes(type)
            || (
                type.FullName != null
                && (
                    type.FullName.StartsWith("api.pieces")
                    || type.FullName.StartsWith("api.models.api")
                )
            )
        );
        BsonSerializer.RegisterSerializer(objectSerializer);
    }
}
