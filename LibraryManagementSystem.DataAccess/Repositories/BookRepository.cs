using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;


namespace LibraryManagementSystem.DataAccess.Repositories
{
    public sealed class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context)
          :base(context)
        {
            _context = context;
        }

        public void CreateBook(Book book) => Create(book);

        public void CreateBookCollection(IEnumerable<Book> books) => CreateCollection(books);

        public async Task<PagedList<Book>> GetAllBooksAsync(PaginationParameters paginationParameters, bool trackChanges)
        {
            var booksQuery = FindAll( trackChanges)
                             .Sort(paginationParameters.OrderBy);

            return await PagedList<Book>.ToPagedList(booksQuery, paginationParameters.PageNumber, paginationParameters.PageSize);
        }

        public async Task<Book?> GetBookByIdAsync(int id, bool trackChanges = true) =>
            await FindByCondition(b => b.Id == id, trackChanges)
             .SingleOrDefaultAsync();

        public async Task<Book?> GetBookByIdIncludeBorrowingsAsync(int id, bool trackChanges = true) =>
           await FindByCondition(b => b.Id == id, trackChanges,_IncludeBorrowingsExpression)
            .SingleOrDefaultAsync();

        public async Task<Book?> GetBookByIdIncludeAuthorsAsync(int id, bool trackChanges = true) =>
            await FindByCondition(b => b.Id == id, trackChanges, _IncludeAuthorsExpression)
             .SingleOrDefaultAsync();

        public async Task<Book?> GetBookByIdIncludeAuthorsAndPersonAsync(int id, bool trackChanges = true) =>
            await FindByCondition(b => b.Id == id, trackChanges, _IncludeAuthorsAndPersonExpression)
             .SingleOrDefaultAsync();

        public async Task<Book?> GetBookByIsbnIncludeAuthorsAndPersonAsync(string isbn, bool trackChanges = true) =>
            await FindByCondition(b => b.ISBN == isbn, trackChanges, _IncludeAuthorsAndPersonExpression)
             .SingleOrDefaultAsync();

       

        public async Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<int> ids, bool trackChanges = true) =>
            await FindByCondition(b =>ids.Contains(b.Id), trackChanges)
            .ToListAsync();

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, bool trackChanges = true) =>
            await FindByCondition(b => b.Authors.Select(a => a.Id).Contains(authorId), trackChanges
            , _IncludeAuthorsExpression)
            .ToListAsync();

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync(bool trackChanges = true) =>
           await FindByCondition(book => book.Borrowings.All(borrowing => borrowing.ReturnDate == null), trackChanges
           , _IncludeBorrowingsExpression)
           .ToListAsync();

        public async Task<PagedList<Book>> SearchBooksByAuthorNameOrTitleAsync
            (SearchParameters searchParameters, bool trackChanges = true)
        {
            var booksQuery =  FindByCondition(GetSearchExpression(searchParameters.SearchTerm)
                                 , false, _IncludeAuthorsAndPersonExpression)
                              .Sort(searchParameters.OrderBy);

            return await PagedList<Book>
                .ToPagedList(booksQuery, searchParameters.PageNumber, searchParameters.PageSize);
        }
            

        public async Task<IEnumerable<string>> GetExistingBookIsbns(IEnumerable<string> isbns, bool trackChanges = true) =>
           await FindByCondition(b => isbns.Contains(b.ISBN), trackChanges)
           .Select(b => b.ISBN)
           .ToListAsync();

        public async Task<bool> IsExist(int id) =>
             await IsExist(b => b.Id == id);

        public async Task<bool> IsExist(string isbn) =>
             await IsExist(b => b.ISBN == isbn);

        public void DeleteBook(Book book) => Delete(book);

        #region Helper 

        private static readonly Func<IQueryable<Book>, IIncludableQueryable<Book, Person>> _IncludeAuthorsAndPersonExpression =
         query => query.Include(book => book.Authors).ThenInclude(author => author.Person);

        private static readonly Func<IQueryable<Book>, IIncludableQueryable<Book, IEnumerable<Borrowing>>> _IncludeBorrowingsExpression =
         query => query.Include(book => book.Borrowings);

        private static readonly Func<IQueryable<Book>, IIncludableQueryable<Book, IEnumerable<Author>>> _IncludeAuthorsExpression =
        query => query.Include(book => book.Authors);


        private Expression<Func<Book, bool>> GetSearchExpression(string searchTerm) =>
            book => book.Title.ToLower().Contains(searchTerm)
                                    || book.Authors
                                        .Any(a => a.Person.Name
                                               .FirstName.ToString().ToLower().Contains(searchTerm)
                                               || a.Person.Name
                                               .LastName.ToString().ToLower().Contains(searchTerm));

        #endregion

    }
}
