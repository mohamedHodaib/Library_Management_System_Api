using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Contract
{
    public interface IUnitOfWork : IDisposable 
    {
        IAuthorRepository AuthorRepository {  get; }
        IBookRepository BookRepository {  get; }
        IBorrowerRepository BorrowerRepository { get; }
        IBorrowingRepository BorrowingRepository {  get; }
        IPersonRepository PersonRepository {  get; }

        Task<int> CompleteAsync();
    }
}
