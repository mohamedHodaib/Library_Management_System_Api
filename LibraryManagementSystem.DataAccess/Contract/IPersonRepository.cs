using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Contract
{
    public interface IPersonRepository : IRepositoryBase<Person>
    {
        void CreatePerson(Person person);
        void CreatePersonCollection(IEnumerable<Person> persons);
        Task<PagedList<Person>> GetAllPersonsAsync(PaginationParameters paginationParameters, bool trackChanges = true);
        Task<IEnumerable<Person>> GetPersonsByIdsAsync(IEnumerable<int> ids, bool trackChanges = true);
        Task<Person?> GetPersonByIdAsync(int id, bool trackChanges = true);
        Task<Person?> GetPersonByIdIncludeBorrowerAuthorUserAsync(int id, bool trackChanges = true);
        Task<IEnumerable<int>> GetValidPersonIds(IEnumerable<int> personIds,bool trackChanges = true);
        Task<bool> IsExist(int id);
        void DeletePerson(Person existingPerson); 
    }
}
