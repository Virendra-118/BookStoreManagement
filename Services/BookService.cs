using BookStoreManagement.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace BookStoreManagement.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;
        public BookService(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];

            var databaseName = configuration["MongoDB:DatabaseName"];

            var collectionName = configuration["MongoDB:CollectionName"];

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);

            _books = database.GetCollection<Book>("Books");

        }

        // Create book
        public async Task CreateBookAsync(Book book)
        {
            book.Id = ObjectId.GenerateNewId().ToString();
            await _books.InsertOneAsync(book);
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _books.Find(book => true).ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(string id)
        {
            return await _books.Find(book => book.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Book>> SearchBooksByTitleAsync(string title)
        {
            var filter = Builders<Book>.Filter.Regex(
                book => book.Title,
                new BsonRegularExpression(title, "i")
            );

            return await _books.Find(filter).ToListAsync();
        }

        public async Task<bool> UpdateBookAsync(string id, Book book)
        {
            book.Id = id;

            var result = await _books.ReplaceOneAsync(b => b.Id == id, book);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteBookAsync(string id)
        {
            var result = await _books.DeleteOneAsync(
                book => book.Id == id
            );

            return result.DeletedCount > 0;


        }
    }
}