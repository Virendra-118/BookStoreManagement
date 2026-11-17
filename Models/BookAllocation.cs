
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreManagement.Models
{
    public class BookAllocation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string BookCopyId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnedAt { get; set; }

        public string Status { get; set; } = "Active";
    }
}
