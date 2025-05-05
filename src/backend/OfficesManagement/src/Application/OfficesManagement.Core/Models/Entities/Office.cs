using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OfficesManagement.Core.Models.ValueObjects;
namespace OfficesManagement.Core.Models.Entities;

public class Office
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; }

    [BsonElement("Location")]
    public Location Location { get; set; } = null!;

    [BsonElement("IsActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("RegistryPhoneNumber")]
    public string RegistryPhoneNumber { get; set; }
}
