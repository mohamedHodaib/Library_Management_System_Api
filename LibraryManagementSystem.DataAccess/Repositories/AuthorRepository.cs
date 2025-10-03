using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace LibraryManagementSystem.DataAccess.Repositories
{
    //make the class sealed to optimize the performance
    public sealed class AuthorRepository : RepositoryBase<Author>, IAuthorRepository
    {
        private readonly AppDbContext _context;
        public AuthorRepository(AppDbContext context)
            :base(context)
        {
            _context = context;
        }



        public void CreateAuthor(Author author) => Create(author);

        public void CreateAuthorCollection(IEnumerable<Author> authors) => CreateCollection(authors);

        public async Task<PagedList<Author>> GetAllAuthorsAsync(PaginationParameters paginationParameters, bool trackChanges)
        {
            var authorsQuery =  FindAll(trackChanges, _includePersonExpression)
                                .Sort(paginationParameters.OrderBy);

           return await PagedList<Author>
                .ToPagedList(authorsQuery, paginationParameters.PageNumber, paginationParameters.PageSize);
        }
            

        public async Task<Author?> GetAuthorByIdAsync(int id, bool trackChanges = true) =>
            await FindByCondition(a => a.Id == id, trackChanges)
             .SingleOrDefaultAsync();

        public async Task<Author?> GetAuthorByIdIncludeBooksAndPersonAsync(int id, bool trackChanges = true) =>
            await FindByCondition(a => a.Id == id, trackChanges, _includeBooksAndPersonExpression)
             .SingleOrDefaultAsync();

        public async Task<Author?> GetAuthorByIdIncludeBooksAsync(int id, bool trackChanges = true) =>
            await FindByCondition(a => a.Id == id, trackChanges, _includeBooksExpression)
             .SingleOrDefaultAsync();

        public async Task<Author?> GetAuthorByIdIncludePersonAsync(int id, bool trackChanges = true) =>
            await FindByCondition(a => a.Id == id, trackChanges, _includePersonExpression)
             .SingleOrDefaultAsync();

        public async Task<Author?> GetAuthorByIdIncludeBooksAndPersonAndBorrowingsAsync(int id, bool trackChanges = true) =>
            await FindByCondition(a => a.Id == id, trackChanges, _includeBooksAndPersonAndBorroweingsExpression)
            .FirstOrDefaultAsync();

        public async Task<IEnumerable<Author>> GetAuthorsByIdsAsync(IEnumerable<int> ids, bool trackChanges = true) =>
            await FindByCondition(a => ids.Contains(a.Id),trackChanges,_includePersonExpression)
            .ToListAsync();

        public async Task<IEnumerable<int>> GetExistingAuthorPersonIds(IEnumerable<int> personIds, bool trackChanges = true) =>
           await FindByCondition(a => personIds.Contains(a.PersonId),trackChanges)
           .Select(a => a.PersonId)
           .ToListAsync();

        public async Task<PagedList<Author>> SearchAuthorsByName(SearchParameters searchParameters, bool trackChanges = true)
        {
            var searchTerm = searchParameters.SearchTerm.Trim().ToLower();

            var authorsQuery = FindByCondition(a => a.Person.Name.FirstName.ToString().ToLower().Contains(searchTerm)
                                         || a.Person.Name.LastName.ToString().ToLower().Contains(searchTerm)
                                        , trackChanges, _includePersonExpression)
                                        .Sort(searchParameters.OrderBy);

            return await PagedList<Author>.ToPagedList(authorsQuery, searchParameters.PageNumber
                                                    , searchParameters.PageSize);
        }


        public async Task<bool> IsExistByAuthorId(int id) =>
           await IsExist(b => b.Id == id);
        public async Task<bool> IsExistByPersonId(int id) =>
             await IsExist(b => b.PersonId == id);


        public void DeleteAuthor(Author author) => Delete(author);


        #region Helper 

        private readonly Func<IQueryable<Author>, IIncludableQueryable<Author, Person>> _includePersonExpression =
         query => query.Include(author => author.Person);


        private readonly Func<IQueryable<Author>, IIncludableQueryable<Author, Person>> _includeBooksAndPersonExpression =
         query => query.Include(author => author.Books).Include(author => author.Person);

        private readonly Func<IQueryable<Author>, IIncludableQueryable<Author, IEnumerable<Book>>> _includeBooksExpression =
         query => query.Include(author => author.Books);

        private readonly Func<IQueryable<Author>, IIncludableQueryable<Author, IEnumerable<Borrowing>>> _includeBooksAndPersonAndBorroweingsExpression =
         query => query.Include(author => author.Person)
                 .Include(author => author.Books)
                 .ThenInclude(book => book.Borrowings);


        #endregion
    }
}
