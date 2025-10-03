using LibraryManagementSystem.DataAccess.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Entities.People
{
    public class Author : IBaseEntity
    {
        public int Id { get; set; }
        public string Biography { get; set; } = null!;


        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
