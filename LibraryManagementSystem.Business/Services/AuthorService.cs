using AutoMapper;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.AuthorDtos;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using LibraryManagementSystem.DataAccess.Shared;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Dtos.BookDtos;


namespace LibraryManagementSystem.Business.Services
{
    public sealed class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region CRUD Methods

        public async Task<AuthorDetailsDto> GetByIdAsync(int id)
        {
            var author = await _unitOfWork.AuthorRepository.GetAuthorByIdIncludeBooksAndPersonAsync(id);

            ThrowNotFoundExceptionIfAuthorNotExist(id, author);

            return _mapper.Map<AuthorDetailsDto>(author);
        }

        public async Task<PagedListDto<AuthorSummaryDto>> GetAllAuthorsAsync(PaginationParameters paginationParameters)
        {
            var authorsWithMetaDataList = await _unitOfWork.AuthorRepository
                .GetAllAuthorsAsync( paginationParameters,false);

            var authorDtos = _mapper.Map<IEnumerable<AuthorSummaryDto>>(authorsWithMetaDataList);

            return new PagedListDto<AuthorSummaryDto> 
                        { Items = authorDtos, MetaData = authorsWithMetaDataList.MetaData };
        }

        public async Task<IEnumerable<AuthorSummaryDto>> GetAuthorsByIdsAsync(IEnumerable<int> Ids)
        {
            var authors = await _unitOfWork.AuthorRepository.GetAuthorsByIdsAsync(Ids,false);

            return _mapper.Map<List<AuthorSummaryDto>>(authors);
        }

        public async Task<AuthorDetailsDto> CreateAsync(CreateAuthorDto newAuthorDto)
        {

            if (!await _unitOfWork.PersonRepository.IsExist(newAuthorDto.PersonId))
            {
                throw new NotFoundException($"Person with id {newAuthorDto.PersonId} not found.");
            }

            if (await _unitOfWork.AuthorRepository.IsExistByPersonId(newAuthorDto.PersonId))
            {
               throw new ConflictException($"The following person id" +
                   $" is already registered as author");
            }


            if (await _unitOfWork.BorrowerRepository.IsExistByPersonId(newAuthorDto.PersonId))
            {
                throw new ConflictException($"The following person id" +
                    $" is already registered as borrower");
            }

            var author = _mapper.Map<Author>(newAuthorDto);

            _unitOfWork.AuthorRepository.CreateAuthor(author);

            await _unitOfWork.CompleteAsync();

            author  = await _unitOfWork.AuthorRepository.GetAuthorByIdIncludePersonAsync(author.Id, false); 

            return _mapper.Map<AuthorDetailsDto>(author);
        }


        public async Task<IEnumerable<AuthorDetailsDto>>
            CreateCollectionAsync(IEnumerable<CreateAuthorDto> newAuthorDtos)
        {
            var authorDtoList = newAuthorDtos.ToList();

            var personIds = authorDtoList.Select(a => a.PersonId).Distinct().ToList();

            if (personIds.Count != authorDtoList.Count)
                throw new BadRequestException("Can not link two authors to the same person.");

            //validate Person IDs If any of them related to author

            var existingAuthorPersonIds = await _unitOfWork.AuthorRepository.GetExistingAuthorPersonIds(personIds);
            if(existingAuthorPersonIds.Any())
            {
                throw new ConflictException($"The following person ids are registered as authors" +
                    $":{string.Join(",",existingAuthorPersonIds)}.");
            }

            //Validate person Ids if any of them not exist 
            var validPersonIds = await _unitOfWork.PersonRepository.GetValidPersonIds(personIds);

            var invalidPersonIds = personIds.Except(validPersonIds);
            if (invalidPersonIds.Any())
            {
                throw new NotFoundException($"The following person ids are not found: {invalidPersonIds}.");
            }

            var authors = _mapper.Map<IEnumerable<Author>>(authorDtoList);

            _unitOfWork.AuthorRepository.CreateAuthorCollection(authors);

            await _unitOfWork.CompleteAsync();


            //Refresh Authors
             authors = await _unitOfWork.AuthorRepository
                .GetAuthorsByIdsAsync(personIds,false);


            return _mapper.Map<IEnumerable<AuthorDetailsDto>>(authors);
        }

        public async Task UpdateAsync(int id, UpdateAuthorDto authorToUpdateDto)
        {
            var author = await _unitOfWork.AuthorRepository.GetAuthorByIdAsync(id);

            ThrowNotFoundExceptionIfAuthorNotExist(id,author);
            
            _mapper.Map(authorToUpdateDto, author);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existingAuthor = await _unitOfWork.AuthorRepository.GetAuthorByIdIncludeBooksAsync(id);

            ThrowNotFoundExceptionIfAuthorNotExist(id, existingAuthor);

            if (existingAuthor.Books.Any())
                throw new ConflictException("Can not delete an author who has associated books.");

            _unitOfWork.AuthorRepository.DeleteAuthor(existingAuthor);
            await _unitOfWork.CompleteAsync();
        }

        #endregion


        #region Business Logic Methods 

        public async Task<PagedListDto<AuthorSummaryDto>> SearchAuthorsByNameAsync(SearchParameters searchParameters)
        {
            var authorsWithMetaDataList = await _unitOfWork.AuthorRepository.SearchAuthorsByName(searchParameters, false);

            var authorDtos = _mapper.Map<IEnumerable<AuthorSummaryDto>>(authorsWithMetaDataList);

            return new PagedListDto<AuthorSummaryDto>
               { Items = authorDtos, MetaData = authorsWithMetaDataList.MetaData };
        }

        public async  Task<AuthorStatsDto> GetAuthorStatisticsAsync(int authorId)
        {
            var author = await _unitOfWork.AuthorRepository
                .GetAuthorByIdIncludeBooksAndPersonAndBorrowingsAsync(authorId,false);

            ThrowNotFoundExceptionIfAuthorNotExist(authorId,author);

            var authorStats = _mapper.Map<AuthorStatsDto>(author);

            return authorStats;
        }

        #endregion



        #region Helper

        private void ThrowNotFoundExceptionIfAuthorNotExist(int id ,Author? author)
        {
            if (author == null)
                throw new NotFoundException($"Author with id {id} not found.");
        }

        #endregion

    }
}
