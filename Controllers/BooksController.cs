using Microsoft.AspNetCore.Authorization;
using BookStoreManagement.Models;
using BookStoreManagement.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BookStoreManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book)
        {
            await _bookService.CreateBookAsync(book);

            return CreatedAtAction(
                nameof(GetBookById),
                new { id = book.Id },
                book
            );
        }

        [Authorize(Roles = "Admin, Customer")]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();

            return Ok(books);
        }

        [Authorize(Roles = "Admin, Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid book ID.");
            }

            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            return Ok(book);
        }

        [Authorize(Roles = "Admin, Customer")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Book title is required.");
            }

            var books = await _bookService.SearchBooksByTitleAsync(title);

            if (books.Count == 0)
            {
                return NotFound("No books found.");
            }

            return Ok(books);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(string id, Book book)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid book ID.");
            }

            var updated = await _bookService.UpdateBookAsync(id, book);

            if (!updated)
            {
                return NotFound("Book not found.");
            }

            return Ok("Book updated successfully.");
        }

        // DELETE: api/books/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid book ID.");
            }

            var deleted = await _bookService.DeleteBookAsync(id);

            if (!deleted)
            {
                return NotFound("Book not found.");
            }

            return Ok("Book deleted successfully.");
        }
    }
}