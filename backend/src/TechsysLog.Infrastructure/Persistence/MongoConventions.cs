using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using TechsysLog.Domain.Common;

namespace TechsysLog.Infrastructure.Persistence;

public static class MongoConventions
{
    private static bool _registered;
    private static readonly object Lock = new();

    public static void Register()
    {
        if (_registered) return;
        lock (Lock)
        {
            if (_registered) return;

            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("TechsysLogConventions", pack, _ => true);

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            });

            _registered = true;
        }
    }
}
