using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.DataAccess.Repositories
{
    public sealed class BorrowingRepository : RepositoryBase<Borrowing>, IBorrowingRepository
    {
        private readonly AppDbContext _context;

        public BorrowingRepository(AppDbContext context)
          : base(context)
        {
            _context = context;
        }

        public async Task<Borrowing?> GetActiveBorrowingAsync(int bookId,int borrowerId,bool trackChanges = true) =>
             await FindByCondition(b => b.BookId == bookId
                                    && b.BorrowerId == borrowerId 
                                    && b.ReturnDate == null
                                    ,trackChanges
                                   ).SingleOrDefaultAsync();

        public async Task<bool> IsHasOverdueBooks(int borrowerId, int dueDays) =>
             await IsExist(b => b.BorrowerId == borrowerId 
                        && b.ReturnDate == null 
                        && DateTime.Now.AddDays(dueDays) < DateTime.Now);
    }
}
