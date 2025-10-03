using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Repositories
{
    public sealed class BorrowerRepository : RepositoryBase<Borrower>, IBorrowerRepository
    {
        private readonly AppDbContext _context;
        public BorrowerRepository(AppDbContext context)
          : base(context)
        {
            _context = context;
        }

        public void CreateBorrower(Borrower borrower) => Create(borrower);

        public void CreateBorrowerCollection(IEnumerable<Borrower> borrowers) => CreateCollection(borrowers);

        public async Task<PagedList<Borrower>> GetAllBorrowersAsync(PaginationParameters paginationParameters, bool trackChanges)
        {
            var borrowersQuery = FindAll(trackChanges,query => query.Include(b => b.Person))
                                .Sort(paginationParameters.OrderBy);

            return await PagedList<Borrower>
                .ToPagedList(borrowersQuery, paginationParameters.PageNumber, paginationParameters.PageSize);
        }

        public async Task<Borrower?> GetBorrowerByIdAsync(int id, bool trackChanges = true) =>
            await FindByCondition(b => b.Id == id, trackChanges)
             .SingleOrDefaultAsync();
        public async Task<Borrower?> GetBorrowerByIdIncludeBorrowingsAndPersonAsync(int id, bool trackChanges = true) =>
          await FindByCondition(b => b.Id == id, trackChanges, _IncludeBorrowingsAndPersonExpression)
           .SingleOrDefaultAsync();

        public async Task<Borrower?> GetBorrowerByIdIncludeBorrowingsAsync(int id, bool trackChanges = true) =>
           await FindByCondition(b => b.Id == id, trackChanges, query => query.Include(b => b.Borrowings))
            .SingleOrDefaultAsync();

        public async Task<Borrower?> GetBorrowerByIdIncludeBorrowingsAndBookAsync(int id, bool trackChanges = true) =>
           await FindByCondition(b => b.Id == id, trackChanges, _includeBorrowingsAndBookExpression)
            .SingleOrDefaultAsync();

        public async Task<IEnumerable<Borrower>> GetBorrowersByIdsAsync(IEnumerable<int> ids, bool trackChanges = true) =>
            await FindByCondition(b => ids.Contains(b.Id), trackChanges, _IncludePersonExpression)
            .ToListAsync();

        public async Task<PagedList<Borrower>> SearchBorrowersByNameAsync(SearchParameters searchParameters, bool trackChanges = true)
        {
            var searchTerm = searchParameters.SearchTerm.ToLower().Trim();

            var borrowersQuery = FindByCondition(a => a.Person.Name.FirstName.ToString().ToLower().Contains(searchTerm)
                                                || a.Person.Name.LastName.ToString().ToLower().Contains(searchTerm)
                                                , true, query => query.Include(b => b.Person))
                                                .Sort(searchParameters.OrderBy);

            return await PagedList<Borrower>
                .ToPagedList(borrowersQuery,searchParameters.PageNumber,searchParameters.PageSize);
        }
            

        public async Task<IEnumerable<int>> GetExistingBorrowerPersonIds(IEnumerable<int> personIds, bool trackChanges = true) =>
            await FindByCondition(a => personIds.Contains(a.PersonId), trackChanges)
            .Select(a => a.PersonId)
            .ToListAsync();

        public async Task<bool> IsExistByBorrowerId(int id) =>
             await IsExist(b => b.Id == id);
        public async Task<bool> IsExistByPersonId(int id) =>
             await IsExist(b => b.PersonId == id);

        public void DeleteBorrower(Borrower borrower) => Delete(borrower);

        #region Helper 

        private static readonly Func<IQueryable<Borrower>, IIncludableQueryable<Borrower, Person>> _IncludePersonExpression =
         query => query.Include(borrower => borrower.Person);

        private static readonly Func<IQueryable<Borrower>, IIncludableQueryable<Borrower, Person>> _IncludeBorrowingsAndPersonExpression =
         query => query.Include(borrower => borrower.Borrowings).Include(borrower => borrower.Person);

        private static readonly Func<IQueryable<Borrower>, IIncludableQueryable<Borrower, Book>> _includeBorrowingsAndBookExpression =
         query => query.Include(borrower => borrower.Borrowings).ThenInclude(borrowing => borrowing.Book);
        #endregion
    }
}
