using LibraryManagementSystem.DataAccess.Contract;

namespace LibraryManagementSystem.DataAccess.Entities.People
{
    public class Person : IBaseEntity
    {
        public int Id { get; set; }

        public PersonName Name { get; set; } = new PersonName();

        public Borrower? Borrower { get; set; }
        public Author? Author { get; set; }
        public User? User { get; set; } 
    }
}