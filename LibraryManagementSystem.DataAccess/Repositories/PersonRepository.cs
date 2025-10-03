using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Extensions;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Repositories
{
    public sealed class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        private readonly AppDbContext _context;
        public PersonRepository(AppDbContext context)
          : base(context)
        {
            _context = context;
        }


        public void CreatePerson(Person person) => Create(person);

        public void CreatePersonCollection(IEnumerable<Person> persons) => CreateCollection(persons);

        public async Task<PagedList<Person>> GetAllPersonsAsync(PaginationParameters paginationParameters, bool trackChanges = true)
        {
            var personsQuery = FindAll(trackChanges)
                                .Sort(paginationParameters.OrderBy);

            return await PagedList<Person>
                .ToPagedList(personsQuery, paginationParameters.PageNumber, paginationParameters.PageSize);
        }

        public async Task<Person?> GetPersonByIdAsync(int id, bool trackChanges) =>
            await FindByCondition(b => b.Id == id, trackChanges)
             .SingleOrDefaultAsync();

        public async Task<IEnumerable<int>> GetValidPersonIds(IEnumerable<int> personIds,bool trackChanges = true) =>
             await FindByCondition(a => personIds.Contains(a.Id),trackChanges)
              .Select(a => a.Id)
              .ToListAsync();

        public async Task<Person?> GetPersonByIdIncludeBorrowerAuthorUserAsync(int id, bool trackChanges = true) =>
           await FindByCondition(b => b.Id == id, trackChanges
               , _includeBorrowerAuthorUserExpression)
            .SingleOrDefaultAsync();

        public async Task<IEnumerable<Person>> GetPersonsByIdsAsync(IEnumerable<int> ids, bool trackChanges = true) =>
            await FindByCondition(b => ids.Contains(b.Id), trackChanges)
            .ToListAsync();

        public async Task<bool> IsExist(int id) =>
             await IsExist(b => b.Id == id);

        public void DeletePerson(Person person) => Delete(person);

        #region Helper 

        private static readonly Func<IQueryable<Person>, IIncludableQueryable<Person, User?>> _includeUserExpression =
         query => query.Include(person => person.User);

        private static readonly Func<IQueryable<Person>, IIncludableQueryable<Person, Author?>> _includeBorrowerAuthorUserExpression =
         query => query.Include(person => person.User)
                  .Include(person => person.Borrower)
                  .ThenInclude(borrower => borrower!.Borrowings)
                  .Include(person => person.Author);
        #endregion

    }
}
