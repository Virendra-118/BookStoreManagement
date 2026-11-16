
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreManagement.Models
{
    public class BookCopy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; } = string.Empty;

        public string CopyNumber { get; set; } = string.Empty;

        public string Status { get; set; } = "Available";
    }
}
