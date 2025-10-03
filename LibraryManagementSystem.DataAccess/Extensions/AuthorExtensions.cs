using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions.Utility;
using System.Linq.Dynamic.Core;

namespace LibraryManagementSystem.DataAccess.Extensions
{
    public static class AuthorExtensions
    {
        public static IQueryable<Author> Sort(this IQueryable<Author> authors,string? orderQueryString)
        {
            if (string.IsNullOrEmpty(orderQueryString))
                return authors.OrderBy(p => p.Person.Name.FirstName).ThenBy(p => p.Person.Name.LastName); 

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Author>(orderQueryString);

            if(string.IsNullOrEmpty(orderQuery))
                return authors.OrderBy(p => p.Person.Name.FirstName).ThenBy(p => p.Person.Name.LastName);

            return authors.OrderBy(orderQuery);
        }
    }
}
