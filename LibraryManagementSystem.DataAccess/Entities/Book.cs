using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities.People;

namespace LibraryManagementSystem.DataAccess.Entities
{
    public class Book : IBaseEntity,ISoftDeletable
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int PublishYear { get; set; } 

        public bool IsDeleted {  get; set; }
        public DateTime? DeleteDate {  get; set; }

        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<Borrower> Borrowers { get; set; } = new List<Borrower>();
        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    }
}