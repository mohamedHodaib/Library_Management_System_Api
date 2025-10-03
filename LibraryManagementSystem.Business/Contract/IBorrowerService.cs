using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.Business.Dtos.BorrowerDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Shared;

namespace LibraryManagementSystem.Business.Contract
{
    public interface IBorrowerService
    {
        #region CRUD Methods

        Task<BorrowerDetailsDto> GetBorrowerByIdAsync(int id);
        Task<PagedListDto<BorrowerSummaryDto>> GetAllBorrowersAsync(PaginationParameters paginationParameters);
        Task<IEnumerable<BorrowerSummaryDto>> GetBorrowersByIdsAsync(IEnumerable<int> Ids);
        Task<BorrowerDetailsDto> CreateBorrowerAsync(CreateBorrowerDto newBorrowerDto);
        Task<IEnumerable<BorrowerDetailsDto>> CreateBorrowerCollectionAsync(IEnumerable<CreateBorrowerDto> newBorrowerDtos);

        Task DeleteBorrowerAsync(int id);

        #endregion


        #region Business Logic Methods

        Task<IEnumerable<BorrowingHistoryDto>> GetBorrowingHistoryAsync(int borrowerId);
        Task<IEnumerable<BookSummaryDto>> GetCurrentLoansAsync(int borrowerId);
        Task<IEnumerable<OverdueLoanDto>> GetOverdueLoansAsync(int borrowerId);
        Task<PagedListDto<BorrowerSummaryDto>> SearchBorrowersByNameAsync(SearchParameters searchParameters);

        #endregion
    }
}
