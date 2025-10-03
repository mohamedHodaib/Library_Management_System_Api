using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions.Utility;
using System.Linq.Dynamic.Core;

namespace LibraryManagementSystem.DataAccess.Extensions
{
    public static class BookExtensions
    {
        public static IQueryable<Book> Sort(this IQueryable<Book> books, string? orderQueryString)
        {
            if (string.IsNullOrEmpty(orderQueryString))
                return books.OrderBy(a => a.Title);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderQueryString);

            if (string.IsNullOrEmpty(orderQuery))
                return books.OrderBy(a => a.Title);


            return books.OrderBy(orderQuery);
        }
    }
}
