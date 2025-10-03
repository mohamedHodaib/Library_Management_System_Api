using LibraryManagementSystem.Business.Validations;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Business.Dtos.BookDtos
{
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(?:ISBN(?:-13)?:? )?(?=[0-9]{13}$|[0-9]{9}X$)([0-9]{1,5}[- ]?){3}[0-9X]$",
        ErrorMessage = "The ISBN format is not valid.")]
        public string ISBN { get; set; } = null!;

        [Required]
        [Range(1000, 9999)] // Assuming a 4-digit year
        public int PublishYear { get; set; }

        [Required]
        [ListElementsRange(1,int.MaxValue)]
        public List<int> AuthorIds { get; set; } = new List<int>();
    }

    public class UpdateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(?:ISBN(?:-13)?:? )?(?=[0-9]{13}$|[0-9]{9}X$)([0-9]{1,5}[- ]?){3}[0-9X]$",
        ErrorMessage = "The ISBN format is not valid.")]
        public string ISBN { get; set; } = null!;

        [Required]
        [Range(1000, 9999)] // Assuming a 4-digit year
        public int PublishYear { get; set; }
    }

    public class BookSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
    }

    public class BookDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int PublishYear { get; set; }

        public List<string> AuthorNames { get; set; } = new List<string>();
    }


    public class BorrowingHistoryDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public DateOnly BorrowDate { get; set; }
        public bool IsReturned { get; set; }
    }


    public class OverdueLoanDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public DateOnly BorrowDate { get; set; }
        public DateOnly DueDate { get; set; }
        public int OverDueDays { get; set; }

    }
}
