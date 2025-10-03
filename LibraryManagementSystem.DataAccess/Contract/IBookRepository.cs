using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Repositories
{
    public interface IBookRepository : IRepositoryBase<Book>
    {   
        void CreateBook(Book borrower);
        void CreateBookCollection(IEnumerable<Book> borrowers);
        Task<PagedList<Book>> GetAllBooksAsync(PaginationParameters paginationParameters, bool trackChanges = true);
        Task<Book?> GetBookByIdAsync(int id, bool trackChanges = true);
        Task<Book?> GetBookByIdIncludeAuthorsAsync(int authorId, bool trackChanges = true);
        Task<Book?> GetBookByIdIncludeBorrowingsAsync(int id, bool trackChanges = true);
        Task<Book?> GetBookByIdIncludeAuthorsAndPersonAsync(int id, bool trackChanges = true);
        Task<Book?> GetBookByIsbnIncludeAuthorsAndPersonAsync(string isbn, bool trackChanges = true);
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, bool trackChanges = true);
        Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<int> ids, bool trackChanges = true);
        Task<IEnumerable<Book>> GetAvailableBooksAsync(bool trackChanges = true);
        Task<PagedList<Book>> SearchBooksByAuthorNameOrTitleAsync(SearchParameters searchParameters ,bool trackChanges = true);
        Task<IEnumerable<string>> GetExistingBookIsbns(IEnumerable<string> allIsbns, bool trackChanges = true);
        Task<bool> IsExist(int id);
        Task<bool> IsExist(string isbn);
        void DeleteBook(Book existingBook);
    }
}
