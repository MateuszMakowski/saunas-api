using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string Role { get; set; }
    // inne pola
}
