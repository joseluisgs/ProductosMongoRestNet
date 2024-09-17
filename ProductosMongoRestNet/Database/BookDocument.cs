using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductosMongoRestNet.Models;

public class BookDocument
{
    [BsonId] // Esto indica que el campo es el identificador de la colección
    [BsonRepresentation(BsonType.ObjectId)] // Esto indica que el campo es de tipo ObjectId
    public string? Id { get; set; } // El identificador de la colección

    [BsonElement("Name")] // Esto indica el nombre del campo en la colección
    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!;
}