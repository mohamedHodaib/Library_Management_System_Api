using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Contract
{
    public interface IAuthorRepository : IRepositoryBase<Author>
    {
        void CreateAuthor(Author author);
        void CreateAuthorCollection(IEnumerable<Author> authors);
        Task<PagedList<Author>> GetAllAuthorsAsync(PaginationParameters paginationParameters, bool trackChanges = true);
        Task<Author?> GetAuthorByIdIncludeBooksAndPersonAndBorrowingsAsync(int id, bool trackChanges = true);
        Task<Author?> GetAuthorByIdAsync(int id, bool trackChanges = true);
        Task<Author?> GetAuthorByIdIncludePersonAsync(int id, bool trackChanges = true);
        Task<Author?> GetAuthorByIdIncludeBooksAsync(int id,bool trackChanges = true);
        Task<Author?> GetAuthorByIdIncludeBooksAndPersonAsync(int id, bool trackChanges = true);
        Task<IEnumerable<Author>> GetAuthorsByIdsAsync(IEnumerable<int> ids, bool trackChanges = true);
        Task <PagedList<Author>> SearchAuthorsByName(SearchParameters searchParameters, bool trackChanges = true);
        Task<IEnumerable<int>> GetExistingAuthorPersonIds(IEnumerable<int> personIds ,bool trackChanges = true);
        Task<bool> IsExistByAuthorId(int id);
        Task<bool> IsExistByPersonId(int id);
        void DeleteAuthor(Author author);

    }
}
