
using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.DTOs
{
    public class AllocateBookRequest
    {
        [Required]
        public string BookCopyId { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }
    }
}
