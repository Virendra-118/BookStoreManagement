
using BookStoreManagement.DTOs;
using BookStoreManagement.Models;
using BookStoreManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class BookAllocationsController : ControllerBase
    {
        private readonly BookAllocationService _allocationService;

        public BookAllocationsController(BookAllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        [HttpPost]
        public async Task<IActionResult> AllocateBook(AllocateBookRequest request)
        {
            var allocation = new BookAllocation
            {
                BookCopyId = request.BookCopyId,
                UserId = request.UserId,
                DueDate = request.DueDate
            };

            var result = await _allocationService.AllocateBookAsync(allocation);

            if (!result)
            {
                return BadRequest("Book copy is unavailable or customer does not exist.");
            }

            return Ok("Book allocated successfully.");
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAllocations()
        {
            var allocations = await _allocationService.GetAllAllocationsAsync();

            return Ok(allocations);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllocationById(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid allocation ID.");
            }

            var allocation = await _allocationService.GetAllocationByIdAsync(id);

            if (allocation == null)
            {
                return NotFound("Allocation not found.");
            }

            return Ok(allocation);
        }


        [Authorize(Roles = "Customer")]
        [HttpGet("my-books")]
        public async Task<IActionResult> GetMyBooks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var allocations = await _allocationService.GetMyAllocationsAsync(userId);

            return Ok(allocations);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnBook(string id)
        {
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid allocation ID.");
            }

            var result = await _allocationService.ReturnBookAsync(id);

            if (!result)
            {
                return NotFound("Active allocation not found.");
            }

            return Ok("Book returned successfully.");
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueBooks()
        {
            var allocations = await _allocationService.GetOverdueAllocationsAsync();

            return Ok(allocations);
        }




    }
}
