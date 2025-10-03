using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities.People;

namespace LibraryManagementSystem.DataAccess.Entities
{
    public class Borrowing : ISoftDeletable,IBaseEntity
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;


        public int BorrowerId { get; set; } 
        public Borrower Borrower { get; set; } = null!;


        public DateOnly BorrowDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        public bool IsReturned => ReturnDate.HasValue;

        public bool IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}