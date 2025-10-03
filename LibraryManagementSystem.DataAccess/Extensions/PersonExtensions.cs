using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions.Utility;
using System.Linq.Dynamic.Core;

namespace LibraryManagementSystem.DataAccess.Extensions
{
    public static class PersonExtensions
    {
        public static IQueryable<Person> Sort(this IQueryable<Person> persons, string? orderQueryString)
        {
            if (string.IsNullOrEmpty(orderQueryString))
                return persons.OrderBy(p => p.Name.FirstName).ThenBy(p => p.Name.LastName);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Person>(orderQueryString);

            if (string.IsNullOrEmpty(orderQuery))
                return persons.OrderBy(p => p.Name.FirstName).ThenBy(p => p.Name.LastName);

            return persons.OrderBy(orderQuery);
        }
    }
}
