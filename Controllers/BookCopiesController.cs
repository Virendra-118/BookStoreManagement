
using BookStoreManagement.Models;
using BookStoreManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class BookCopiesController : ControllerBase
    {
        private readonly BookCopyService _bookCopyService;

        public BookCopiesController(BookCopyService bookCopyService)
        {
            _bookCopyService = bookCopyService;
        }

        [HttpPost]
        public async Task<IActionResult> AddBookCopy(BookCopy bookCopy)
        {
            await _bookCopyService.AddBookCopyAsync(bookCopy);

            return Ok("Book copy added successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookCopies()
        {
            var bookCopies = await _bookCopyService.GetAllBookCopiesAsync();

            return Ok(bookCopies);
        }
    }
}
