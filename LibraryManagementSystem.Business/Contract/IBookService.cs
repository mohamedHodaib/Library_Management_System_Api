using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.Business.Dtos.BorrowingDtos;
using LibraryManagementSystem.Business.Dtos.Shared;


namespace LibraryManagementSystem.Business.Contract
{
    public interface IBookService
    {
        #region CRUD Methods

        Task<BookDetailsDto?> GetBookByIdAsync(int id);
        Task<PagedListDto<BookSummaryDto>> GetAllBooksAsync(PaginationParameters paginationParameters);
        Task<IEnumerable<BookSummaryDto>> GetBooksByIdsAsync(IEnumerable<int> Ids);
        Task<BookDetailsDto> CreateBookAsync(CreateBookDto newBookDto);
        Task<IEnumerable<BookDetailsDto>> CreateBooksCollectionAsync(IEnumerable<CreateBookDto> newBookDtos);

        Task UpdateAsync(int id, UpdateBookDto bookToUpdateDto);
        Task DeleteAsync(int id);

        #endregion


        #region Business Logic Methods

        Task<BookDetailsDto> GetByIsbnAsync(string isbn);
        Task<UpdateBookDto> GetUpdateBookDto(int id);
        Task<IEnumerable<BookSummaryDto>> GetBooksByAuthorIdAsync(int authorId);
        Task<IEnumerable<BookSummaryDto>> GetAvailableBooksAsync();
        Task<PagedListDto<BookSummaryDto>> SearchBooksByAuthorNameOrTitleAsync(SearchParameters searchParameters);
        Task<BookDetailsDto?> AddAuthorToBookAsync(int bookId, int authorId);
        Task<BorrowingDetailsDto> BorrowBookAsync(int bookId, string userId);
        Task ReturnBookAsync(int bookId,string userId);

        #endregion
    }
}
