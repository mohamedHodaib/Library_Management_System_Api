using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions.Utility;
using System.Linq.Dynamic.Core;

namespace LibraryManagementSystem.DataAccess.Extensions
{
    public static class BorrowerExtensions
    {
        public static IQueryable<Borrower> Sort(this IQueryable<Borrower> borrowers, string? orderQueryString)
        {
            if (string.IsNullOrEmpty(orderQueryString))
                return borrowers.OrderBy(p => p.Person.Name.FirstName).ThenBy(p => p.Person.Name.LastName);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Borrower>(orderQueryString);

            if (string.IsNullOrEmpty(orderQuery))
                return borrowers.OrderBy(p => p.Person.Name.FirstName).ThenBy(p => p.Person.Name.LastName);

            return borrowers.OrderBy(orderQuery);
        }
    }
}
