using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Core.Models.ValueObjects;

namespace OfficesManagement.Infrastructure.Persistence.Contexts
{
    public static class MongoMappings
    {
        public static void Register()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("OfficesManagementConventions", pack, t => true);

            if (!BsonClassMap.IsClassMapRegistered(typeof(Location)))
            {
                BsonClassMap.RegisterClassMap<Location>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(c => c.Address).SetElementName("address");
                    cm.MapMember(c => c.City).SetElementName("city");
                    cm.MapMember(c => c.Country).SetElementName("country");
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Office)))
            {
                BsonClassMap.RegisterClassMap<Office>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id)
                      .SetElementName("_id")
                      .SetSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.BsonType.String));

                    cm.MapMember(c => c.Name).SetElementName("name");
                    cm.MapMember(c => c.RegistryPhoneNumber).SetElementName("registryPhoneNumber");
                    cm.MapMember(c => c.IsActive).SetElementName("isActive");
                    cm.MapMember(c => c.Location).SetElementName("location");

                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
