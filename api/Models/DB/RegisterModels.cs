using System.Reflection;
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

        var assembly = Assembly.GetAssembly(typeof(IPiece));
        var types = assembly
            ?.GetTypes()
            .Where(type => !type.IsInterface && typeof(IPiece).IsAssignableFrom(type));

        if (types is not null)
        {
            foreach (var t in types)
            {
                if (t is not null)
                {
                    BsonClassMap.LookupClassMap(t);
                }
            }
        }

        BsonSerializer.RegisterSerializer(objectSerializer);
    }
}
