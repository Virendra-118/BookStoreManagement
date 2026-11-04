using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreManagement.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public DateTime PublishedDate { get; set; }

        public string? CoverImageUrl { get; set; }
    }
}
