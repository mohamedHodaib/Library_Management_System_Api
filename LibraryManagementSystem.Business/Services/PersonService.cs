using AutoMapper;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.AuthorDtos;
using LibraryManagementSystem.Business.Dtos.PersonDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.AspNetCore.Identity;


namespace LibraryManagementSystem.Business.Services
{
    public sealed class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public PersonService(IUnitOfWork unitOfWork, IMapper mapper,UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        

        #region CRUD Methods

        public async Task<PersonDetailsDto> GetPersonByIdAsync(int id)
        {
            var person = await _unitOfWork.PersonRepository
                                .GetPersonByIdIncludeBorrowerAuthorUserAsync(id,false);

            ThrowNotFoundExceptionIfPersonNotExist(id, person);

            var personDetails = _mapper.Map<PersonDetailsDto>(person);

            personDetails.Role = GetRole(person);

            return personDetails;
        }



        public async Task<PagedListDto<PersonSummaryDto>> GetAllPersonsAsync(PaginationParameters paginationParameters)
        {
            var personsWithMetaDataList = await _unitOfWork.PersonRepository.GetAllPersonsAsync(paginationParameters, false);

            var personDtos = _mapper.Map<IEnumerable<PersonSummaryDto>>(personsWithMetaDataList);

            return new PagedListDto<PersonSummaryDto>
               { Items = personDtos, MetaData = personsWithMetaDataList.MetaData };
        }


        public async Task<IEnumerable<PersonSummaryDto>> GetPersonsByIdsAsync(IEnumerable<int> Ids)
        {
            var people = await _unitOfWork.PersonRepository.GetPersonsByIdsAsync(Ids,false);

            return _mapper.Map<List<PersonSummaryDto>>(people);
        }


        public async Task<PersonDetailsDto> CreatePersonAsync(CreatePersonDto dto)
        {
            var person = _mapper.Map<Person>(dto);

            _unitOfWork.PersonRepository.CreatePerson(person);
            await _unitOfWork.CompleteAsync();

            var personDetails = _mapper.Map<PersonDetailsDto>(person);

            return personDetails;
        }


        public async Task<IEnumerable<PersonDetailsDto>> CreatePersonsCollectionAsync(IEnumerable<CreatePersonDto> newPersonDtos)
        {

            var people = _mapper.Map<IEnumerable<Person>>(newPersonDtos);

            _unitOfWork.PersonRepository.CreatePersonCollection(people);
            await _unitOfWork.CompleteAsync();
            
            var peopleDetails = _mapper.Map<IEnumerable<PersonDetailsDto>>(people);

            return peopleDetails;
        }


        public async Task UpdateAsync(int id, UpdatePersonDto personToUpdateDto)
        {
            var person = await _unitOfWork.PersonRepository.GetPersonByIdAsync(id);

            ThrowNotFoundExceptionIfPersonNotExist(id, person);

            _mapper.Map(personToUpdateDto, person);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var existingPerson = await _unitOfWork.PersonRepository
                                        .GetPersonByIdIncludeBorrowerAuthorUserAsync(id,true);

            ThrowNotFoundExceptionIfPersonNotExist(id, existingPerson);

            if (existingPerson.Borrower?.Borrowings.Any(br => !br.IsReturned) == true)
                throw new ConflictException("Can not delete person with active loans.");

            if(existingPerson.User != null)
            {
                var IdenityResult = await _userManager.DeleteAsync(existingPerson.User);

                if (!IdenityResult.Succeeded)
                    throw new Exception("Failed to delete user account.");
            }


            _unitOfWork.PersonRepository.DeletePerson(existingPerson);
            await _unitOfWork.CompleteAsync();
        }

        #endregion


        #region helper 

        private string GetRole(Person person)
        {
            if (person.Author != null) return "Author";
            if (person.Borrower != null) return "Borrower";
            if (person.User != null) return "Admin";

            return "Not assigned";
        }

        private void ThrowNotFoundExceptionIfPersonNotExist(int id, Person? person)
        {
            if (person == null)
                throw new NotFoundException($"Person with id {id} not found.");
        }

        #endregion
    }
}
