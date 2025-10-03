using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.DataAccess.Contract
{
    public interface IBorrowerRepository : IRepositoryBase<Borrower>
    {
        void CreateBorrower(Borrower borrower);
        void CreateBorrowerCollection(IEnumerable<Borrower> borrowers);
        Task<PagedList<Borrower>> GetAllBorrowersAsync(PaginationParameters paginationParameters, bool trackChanges = true);
        Task<IEnumerable<Borrower>> GetBorrowersByIdsAsync(IEnumerable<int> ids, bool trackChanges = true);
        Task<Borrower?> GetBorrowerByIdAsync(int id, bool trackChanges = true);
        Task<Borrower?> GetBorrowerByIdIncludeBorrowingsAsync(int id, bool trackChanges = true);
        Task<Borrower?> GetBorrowerByIdIncludeBorrowingsAndPersonAsync(int id, bool trackChanges = true);
        Task<Borrower?> GetBorrowerByIdIncludeBorrowingsAndBookAsync(int id, bool trackChanges = true);
        Task<PagedList<Borrower>> SearchBorrowersByNameAsync(SearchParameters searchParameters, bool trackChanges = true);
        Task<IEnumerable<int>> GetExistingBorrowerPersonIds(IEnumerable<int> personIds, bool trackChanges = true);
        Task<bool> IsExistByBorrowerId(int id);
        Task<bool> IsExistByPersonId(int id);
        void DeleteBorrower(Borrower existingBorrower); 
    }
}
