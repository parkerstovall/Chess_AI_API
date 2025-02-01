using ChessApi.HelperClasses.Chess;
using ChessApi.Models.API;
using ChessApi.Pieces;
using ChessApi.Pieces.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace ChessApi.Models.DB;

public static class DatabaseInit
{
    public static void RegisterModels()
    {
        var objectSerializer = new ObjectSerializer(type =>
            ObjectSerializer.DefaultAllowedTypes(type)
            || (
                type.FullName is not null
                && (
                    type.FullName.StartsWith("ChessApi.Pieces")
                    || type.FullName.StartsWith("ChessApi.Models.API")
                )
            )
        );
        BsonSerializer.RegisterSerializer(objectSerializer);
    }
}
