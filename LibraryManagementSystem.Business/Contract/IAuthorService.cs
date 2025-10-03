using LibraryManagementSystem.Business.Dtos.AuthorDtos;
using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Contract
{
    public interface IAuthorService
    {
        #region CRUD Methods

        Task<AuthorDetailsDto> GetByIdAsync(int id);

        Task<PagedListDto<AuthorSummaryDto>> GetAllAuthorsAsync(PaginationParameters paginationParameters);

        Task<IEnumerable<AuthorSummaryDto>> GetAuthorsByIdsAsync(IEnumerable<int> Ids);

        Task<AuthorDetailsDto> CreateAsync(CreateAuthorDto newAuthorDto);


        Task<IEnumerable<AuthorDetailsDto>> CreateCollectionAsync(IEnumerable<CreateAuthorDto> newAuthorDtos);

        Task UpdateAsync(int id, UpdateAuthorDto authorToUpdateDto);

        Task DeleteAsync(int id);

        #endregion

        // Business Logic Methods
        Task<PagedListDto<AuthorSummaryDto>> SearchAuthorsByNameAsync(SearchParameters searchParameters);
        Task<AuthorStatsDto> GetAuthorStatisticsAsync(int authorId);
    }
}
