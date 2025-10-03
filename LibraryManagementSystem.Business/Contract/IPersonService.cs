using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.Business.Dtos.BorrowerDtos;
using LibraryManagementSystem.Business.Dtos.PersonDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Business.Contract
{
    public interface IPersonService
    {
        Task<PersonDetailsDto> GetPersonByIdAsync(int id);
        Task<PagedListDto<PersonSummaryDto>> GetAllPersonsAsync(PaginationParameters paginationParameters);
        Task<IEnumerable<PersonSummaryDto>> GetPersonsByIdsAsync(IEnumerable<int> Ids);
        Task<PersonDetailsDto> CreatePersonAsync(CreatePersonDto createPersonDto);
        Task<IEnumerable<PersonDetailsDto>> CreatePersonsCollectionAsync(IEnumerable<CreatePersonDto> createDtos);
        Task UpdateAsync(int id, UpdatePersonDto personToUpdateDto);
        Task DeletePersonAsync(int id);
    }
}
