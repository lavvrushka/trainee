using MongoDB.Bson.Serialization.Attributes;
namespace OfficesManagement.Core.Models.ValueObjects;

public class Location
{
    [BsonElement("Address")]
    public string Address { get; set; } = null!;

    [BsonElement("City")]
    public string City { get; set; } = null!;

    [BsonElement("Country")]
    public string Country { get; set; } = null!;
}
