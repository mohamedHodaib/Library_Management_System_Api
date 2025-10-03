using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Contract
{
    public interface IBorrowingRepository : IRepositoryBase<Borrowing>
    {
        Task<Borrowing?> GetActiveBorrowingAsync(int bookId,int borrowerId,bool trackChanges = true);
        Task<bool> IsHasOverdueBooks(int borrowerId,int dueDays);
    }
}
