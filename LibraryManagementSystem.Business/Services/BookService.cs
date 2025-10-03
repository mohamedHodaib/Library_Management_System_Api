using AutoMapper;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.AuthorDtos;
using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.Business.Dtos.BorrowingDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.Business.Options;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using System.Net;

namespace LibraryManagementSystem.Business.Services
{
    public sealed class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly LoanSettings _loanSettings;
        private readonly UserManager<User> _userManager;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper,IOptionsMonitor<LoanSettings> loanSettingsOptions
            ,UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loanSettings = loanSettingsOptions.CurrentValue;
            _userManager = userManager;
        }


        #region CRUD Methods

        public async Task<BookDetailsDto?> GetBookByIdAsync(int id)
        {
            var book = await _unitOfWork.BookRepository.GetBookByIdIncludeAuthorsAndPersonAsync(id,false);

            ThrowNotFoundExceptionIfBookNotExist(id, book);

            return _mapper.Map<BookDetailsDto>(book);
        }

        public async Task<PagedListDto<BookSummaryDto>>
            GetAllBooksAsync(PaginationParameters paginationParameters)
        {
            var booksWithMetaDataList = await _unitOfWork.BookRepository
                                        .GetAllBooksAsync(paginationParameters, false);

            var bookDtos = _mapper.Map<IEnumerable<BookSummaryDto>>(booksWithMetaDataList);

            return new PagedListDto<BookSummaryDto>
                { Items = bookDtos, MetaData = booksWithMetaDataList.MetaData };
        }

        public async Task<IEnumerable<BookSummaryDto>> GetBooksByIdsAsync(IEnumerable<int> Ids)
        {
            var books = await _unitOfWork.BookRepository.GetBooksByIdsAsync(Ids,false);

            return _mapper.Map<List<BookSummaryDto>>(books);
        }


        public async Task<BookDetailsDto> CreateBookAsync(CreateBookDto newBookDto)
        {

            if (await _unitOfWork.BookRepository.IsExist(newBookDto.ISBN))
            {
                throw new ConflictException($"A book with ISBN {newBookDto.ISBN} already exist.");
            }


            var book = _mapper.Map<Book>(newBookDto);

            //Validate That All Authors are exist
            var allAuthorIds = newBookDto.AuthorIds.ToList();
            var existingAuthors = await _unitOfWork.AuthorRepository.GetAuthorsByIdsAsync(allAuthorIds,false);

            var notValidAuthorIds = allAuthorIds.Except(existingAuthors.Select(a => a.Id));
            if (notValidAuthorIds.Any())
            {
                throw new NotFoundException($"The following author Ids Are not Found: {notValidAuthorIds}");
            }

            book.Authors = existingAuthors.ToList();

            _unitOfWork.BookRepository.CreateBook(book);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookDetailsDto>(book);
        }


        public async Task<IEnumerable<BookDetailsDto>>
            CreateBooksCollectionAsync(IEnumerable<CreateBookDto> newBookDtos)
        {
            //Validate All Isbns
            var allIsbns = newBookDtos.Select(b => b.ISBN);

            var existingBookIsbns = await _unitOfWork.BookRepository.GetExistingBookIsbns(allIsbns,false);

            if (existingBookIsbns.Any())
            {
                throw new ConflictException($"The following Books Isbns are Already Exist" +
                    $": {string.Join(",", existingBookIsbns)}.");
            }

            //Validate That All Authors are exist
            var allAuthorIds = newBookDtos.SelectMany(b => b.AuthorIds);
            var existingAuthors = await _unitOfWork.AuthorRepository.GetAuthorsByIdsAsync(allAuthorIds,true);

            var notValidAuthorIds = allAuthorIds.Except(existingAuthors.Select(a => a.Id));
            if(notValidAuthorIds.Any())
            {
                throw new NotFoundException($"The following author Ids Are not Found: {notValidAuthorIds}") ;
            }

            var books = new List<Book>();   
            foreach(var dto in newBookDtos)
            {
                var book = _mapper.Map<Book>(dto);

                //Find the authors in the authors list that we are already add it 
                book.Authors = existingAuthors
                               .Where(a => dto.AuthorIds.Contains(a.Id))
                               .ToList();

                books.Add(book);
            }

            _unitOfWork.BookRepository.CreateBookCollection(books);

            await _unitOfWork.CompleteAsync();


            return _mapper.Map<IEnumerable<BookDetailsDto>>(books);
        }


        public async Task UpdateAsync(int id, UpdateBookDto bookToUpdateDto)
        {
            var existingBook = await _unitOfWork.BookRepository.GetBookByIdAsync(id);

            ThrowNotFoundExceptionIfBookNotExist(id, existingBook);

            _mapper.Map(bookToUpdateDto, existingBook);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existingBook = await _unitOfWork.BookRepository.GetBookByIdIncludeBorrowingsAsync(id,true);

            ThrowNotFoundExceptionIfBookNotExist(id, existingBook);

            if (existingBook.Borrowings.Any(br => !br.IsReturned))
                throw new ConflictException("Can not delete book that has an active borrowings");

            _unitOfWork.BookRepository.DeleteBook(existingBook);
            await _unitOfWork.CompleteAsync();
        }

        #endregion





        #region Business Logic Methods

        public async Task<BookDetailsDto> GetByIsbnAsync(string isbn)
        {
            var book = await _unitOfWork.BookRepository.GetBookByIsbnIncludeAuthorsAndPersonAsync(isbn, false);

            if (book == null) throw new NotFoundException($"Book with isbn {isbn} not found.");

            return _mapper.Map<BookDetailsDto>(book);
        }

        public async Task<UpdateBookDto> GetUpdateBookDto(int id)
        {
            var book = await _unitOfWork.BookRepository.GetBookByIdAsync(id);

            ThrowNotFoundExceptionIfBookNotExist(id, book);

            return _mapper.Map<UpdateBookDto>(book);
        }

        public async Task<IEnumerable<BookSummaryDto>> GetBooksByAuthorIdAsync(int authorId)
        {
            if (!await _unitOfWork.AuthorRepository.IsExistByAuthorId(authorId))
                throw new NotFoundException($"Author with id {authorId} not found.");

            var books = await _unitOfWork.BookRepository.GetBooksByAuthorIdAsync(authorId ,false);

            return _mapper.Map<List<BookSummaryDto>>(books);
        }

        public async Task<IEnumerable<BookSummaryDto>> GetAvailableBooksAsync()
        {
            var books = await _unitOfWork.BookRepository.GetAvailableBooksAsync(false);


            return _mapper.Map<List<BookSummaryDto>>(books);
        }

        public async Task<PagedListDto<BookSummaryDto>> SearchBooksByAuthorNameOrTitleAsync
                (SearchParameters searchParameters)
        {
            var booksWithMetaDataList = await _unitOfWork.BookRepository.SearchBooksByAuthorNameOrTitleAsync(searchParameters, false); ;

            var bookDtos = _mapper.Map<IEnumerable<BookSummaryDto>>(booksWithMetaDataList);

            return new PagedListDto<BookSummaryDto>
            { Items = bookDtos, MetaData = booksWithMetaDataList.MetaData };
        }

        public async Task<BookDetailsDto?> AddAuthorToBookAsync(int bookId, int authorId)
        {
            var author = await _unitOfWork.AuthorRepository.GetAuthorByIdIncludePersonAsync(authorId);
            if (author == null) throw new NotFoundException($"Author with id {authorId} not found.");

            var book = await _unitOfWork.BookRepository.GetBookByIdIncludeAuthorsAndPersonAsync(bookId,true);
            ThrowNotFoundExceptionIfBookNotExist(bookId, book);

            if (book.Authors.Any(author => author.Id == authorId))
                return null;

            book.Authors.Add(author);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookDetailsDto>(book);
        }


        public async Task<BorrowingDetailsDto> BorrowBookAsync(int bookId, string userId)
        {
            // OPTIMIZATION: Fetch entities once instead of multiple IsExist checks
            var book = await _unitOfWork.BookRepository.GetBookByIdIncludeBorrowingsAsync(bookId);

            ThrowNotFoundExceptionIfBookNotExist(bookId, book);

            if ( book.Borrowings.Any(borrowing => !borrowing.IsReturned))
                throw new ConflictException($"Book With Id {bookId} Already loaned");

            var user = await _userManager.Users
            .Include(u => u.Person)
            .ThenInclude(p => p.Borrower)
            .ThenInclude(b => b!.Borrowings)
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException($"User with id {userId} Not Found");

            var borrower = user.Person?.Borrower;

            var borrowerId = borrower!.Id;

            if (await _unitOfWork.BorrowingRepository.IsHasOverdueBooks(borrowerId,_loanSettings.DueDays))
                throw new BadRequestException($"Borrow with Id {borrowerId} have overdue not returned books");

            if (borrower.Borrowings.Count == _loanSettings.LoansLimit)
                throw new BadRequestException($"Borrower can't borrow more because " +
                    $"he reaches the limit of loans {_loanSettings.LoansLimit}");

            var borrowing = new Borrowing
            {
                BookId = bookId,
                BorrowerId = borrowerId,
                BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow.Date)
            };

           book.Borrowings.Add(borrowing);
           await _unitOfWork.CompleteAsync();

            return new BorrowingDetailsDto
            {
                Id = borrowing.Id
                ,
                BookTitle = book.Title
                ,
                BorrowerName = user.Person!.Name.ToString()
                ,
                BorrowingDate = borrowing.BorrowDate
                ,
                DueDate =  borrowing.BorrowDate.AddDays(_loanSettings.DueDays)
            };
        }


        public async Task ReturnBookAsync(int bookId,string userId)
        {
            if (!await _unitOfWork.BookRepository.IsExist(bookId))
                throw new NotFoundException($"Book with id {bookId} not found.");

            var user = await _userManager.Users
            .Include(u => u.Person.Borrower)
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException($"User with id {userId} Not Found");

            var borrowerId = user.Person.Borrower!.Id;

            var activeBorrowing = await _unitOfWork.BorrowingRepository.GetActiveBorrowingAsync(bookId,borrowerId);

            if (activeBorrowing == null)
            {
                throw new BadRequestException("This Book is not Borrowed by this user or It already returned.");
            }

            activeBorrowing.ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            await _unitOfWork.CompleteAsync();

        }

        #endregion


        #region Helper

        private void ThrowNotFoundExceptionIfBookNotExist(int id, Book? book)
        {
            if (book == null)
                throw new NotFoundException($"Book with id {id} not found.");
        }

        #endregion

    }
}
