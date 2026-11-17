
using BookStoreManagement.Config;
using BookStoreManagement.Models;
using MongoDB.Driver;

namespace BookStoreManagement.Services
{
    public class BookAllocationService
    {
        private readonly IMongoCollection<BookAllocation> _allocations;
        private readonly IMongoCollection<BookCopy> _bookCopies;
        private readonly IMongoCollection<User> _users;

        public BookAllocationService(MongoDbConfig mongoDbConfig)
        {
            var database = mongoDbConfig.Database;

            _allocations = database.GetCollection<BookAllocation>("BookAllocations");
            _bookCopies = database.GetCollection<BookCopy>("BookCopies");
            _users = database.GetCollection<User>("Users");
        }

        public async Task<bool> AllocateBookAsync(BookAllocation allocation)
        {
            var bookCopy = await _bookCopies.Find(
                x => x.Id == allocation.BookCopyId
            ).FirstOrDefaultAsync();

            if (bookCopy == null || bookCopy.Status != "Available")
            {
                return false;
            }

            var user = await _users.Find(
                x => x.Id == allocation.UserId && x.Role == "Customer"
            ).FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            allocation.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            allocation.IssuedAt = DateTime.UtcNow;
            allocation.Status = "Active";

            await _allocations.InsertOneAsync(allocation);

            var update = Builders<BookCopy>.Update
                .Set(x => x.Status, "Issued");

            await _bookCopies.UpdateOneAsync(
                x => x.Id == allocation.BookCopyId,
                update
            );

            return true;
        }


        public async Task<List<BookAllocation>> GetAllAllocationsAsync()
        {
            return await _allocations.Find(_ => true).ToListAsync();
        }


        public async Task<BookAllocation?> GetAllocationByIdAsync(string id)
        {
            return await _allocations.Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }


        public async Task<List<BookAllocation>> GetMyAllocationsAsync(string userId)
        {
            return await _allocations.Find(x => x.UserId == userId)
                .ToListAsync();
        }


        public async Task<bool> ReturnBookAsync(string id)
        {
            var allocation = await _allocations.Find(
                x => x.Id == id && x.Status == "Active"
            ).FirstOrDefaultAsync();

            if (allocation == null)
            {
                return false;
            }

            var updateAllocation = Builders<BookAllocation>.Update
                .Set(x => x.Status, "Returned")
                .Set(x => x.ReturnedAt, DateTime.UtcNow);

            await _allocations.UpdateOneAsync(
                x => x.Id == id,
                updateAllocation
            );

            var updateBookCopy = Builders<BookCopy>.Update
                .Set(x => x.Status, "Available");

            await _bookCopies.UpdateOneAsync(
                x => x.Id == allocation.BookCopyId,
                updateBookCopy
            );

            return true;
        }


        public async Task<List<BookAllocation>> GetOverdueAllocationsAsync()
        {
            var currentDate = DateTime.UtcNow;

            return await _allocations.Find(
                x => x.Status == "Active" && x.DueDate < currentDate
            ).ToListAsync();
        }



    }
}
