
using BookStoreManagement.Config;
using BookStoreManagement.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreManagement.Services
{
    public class BookCopyService
    {
        private readonly IMongoCollection<BookCopy> _bookCopies;

        public BookCopyService(MongoDbConfig mongoDbConfig)
        {
            _bookCopies = mongoDbConfig.Database
                .GetCollection<BookCopy>("BookCopies");
        }

        public async Task AddBookCopyAsync(BookCopy bookCopy)
        {
            bookCopy.Id = ObjectId.GenerateNewId().ToString();

            await _bookCopies.InsertOneAsync(bookCopy);
        }

        public async Task<List<BookCopy>> GetAllBookCopiesAsync()
        {
            return await _bookCopies.Find(_ => true).ToListAsync();
        }
    }
}
