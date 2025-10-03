using AutoMapper;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.BorrowerDtos;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.Business.Dtos.BookDtos;
using Microsoft.Extensions.Options;
using LibraryManagementSystem.Business.Options;
using LibraryManagementSystem.DataAccess.Shared;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Dtos.AuthorDtos;

namespace LibraryManagementSystem.Business.Services
{
    public sealed class BorrowerService : IBorrowerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly LoanSettings _loanSettings;

        public BorrowerService(IUnitOfWork unitOfWork, IMapper mapper,IOptionsMonitor<LoanSettings> loanSettingsOptions)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loanSettings = loanSettingsOptions.CurrentValue;
        }

        #region CRUD Methods

        public async Task<BorrowerDetailsDto> GetBorrowerByIdAsync(int id)
        {
            var borrower = await _unitOfWork.BorrowerRepository
                            .GetBorrowerByIdIncludeBorrowingsAndPersonAsync(id,false);

            ThrowNotFoundExceptionIfBorrowerNotExist(id, borrower);

            return _mapper.Map<BorrowerDetailsDto>(borrower);   
        }

        public async Task<PagedListDto<BorrowerSummaryDto>> GetAllBorrowersAsync(PaginationParameters paginationParameters)
        {
            var borrowersWithMetaDataList = await _unitOfWork.BorrowerRepository
                .GetAllBorrowersAsync( paginationParameters,false);

            var borrowerDtos = _mapper.Map<IEnumerable<BorrowerSummaryDto>>(borrowersWithMetaDataList);

            return new PagedListDto<BorrowerSummaryDto>
            { Items = borrowerDtos, MetaData = borrowersWithMetaDataList.MetaData };
        }

        public async Task<IEnumerable<BorrowerSummaryDto>> GetBorrowersByIdsAsync(IEnumerable<int> Ids)
        {
            var borrowers = await _unitOfWork.BorrowerRepository.GetBorrowersByIdsAsync(Ids,false);

            return _mapper.Map<List<BorrowerSummaryDto>>(borrowers);
        }


        public async Task<BorrowerDetailsDto> CreateBorrowerAsync(CreateBorrowerDto newBorrowerDto)
        {
            if (!await _unitOfWork.PersonRepository.IsExist(newBorrowerDto.PersonId))
            {
                throw new ConflictException($"Person With id {newBorrowerDto.PersonId} not found.");
            }


            if (await _unitOfWork.BorrowerRepository.IsExistByBorrowerId(newBorrowerDto.PersonId))
            {
                throw new ConflictException($"The following person id" +
                    $" is already registered as Borrower:{newBorrowerDto.PersonId}");
            }

            var borrower = _mapper.Map<Borrower>(newBorrowerDto);

            _unitOfWork.BorrowerRepository.CreateBorrower(borrower);

            await _unitOfWork.CompleteAsync();

            //referesh borrowerInfo
            var newBorrower = _unitOfWork.BorrowerRepository.GetBorrowerByIdIncludeBorrowingsAndPersonAsync(borrower.Id,false);  

            return _mapper.Map<BorrowerDetailsDto>(borrower);
        }


        public async Task<IEnumerable<BorrowerDetailsDto>> 
            CreateBorrowerCollectionAsync(IEnumerable<CreateBorrowerDto> newBorrowerDtos)
        {

            var borrowerDtoList = newBorrowerDtos.ToList();

            var personIds = borrowerDtoList.Select(a => a.PersonId).Distinct().ToList();

            if (personIds.Count != borrowerDtoList.Count)
                throw new BadRequestException("Can not link two borrowers to the same person.");

            //validate Person IDs If any of them related to borrower
            var existingBorrowerPersonIds = await _unitOfWork.BorrowerRepository.GetExistingBorrowerPersonIds(personIds,false);
            if (existingBorrowerPersonIds.Any())
            {
                throw new ConflictException($"The following person ids are registered as borrowers" +
                    $":{string.Join(",", existingBorrowerPersonIds)}.");
            }

            //Validate person Ids if any of them not exist 
            var validPersonIds = await _unitOfWork.PersonRepository.GetValidPersonIds(personIds,false);

            var invalidPersonIds = personIds.Except(validPersonIds);
            if (invalidPersonIds.Any())
            {
                throw new NotFoundException($"The following person ids are not found: {invalidPersonIds}.");
            }

            var borrowers = _mapper.Map<IEnumerable<Borrower>>(borrowerDtoList);

            _unitOfWork.BorrowerRepository.CreateCollection(borrowers);

            await _unitOfWork.CompleteAsync();


            //Refresh Borrowers
            var borrowerIds = borrowers.Select(b => b.Id);
            borrowers = await _unitOfWork.BorrowerRepository.GetBorrowersByIdsAsync(borrowerIds, false);


            return _mapper.Map<IEnumerable<BorrowerDetailsDto>>(borrowers);
        }


        public async Task DeleteBorrowerAsync(int id)
        {
            var existingBorrower = await _unitOfWork.BorrowerRepository
                                     .GetBorrowerByIdIncludeBorrowingsAsync(id,true);

            ThrowNotFoundExceptionIfBorrowerNotExist(id, existingBorrower);

            if (existingBorrower.Borrowings.Any(br => !br.IsReturned))
                throw new ConflictException("Can not delete borrower that has an active borrowings");

            _unitOfWork.BorrowerRepository.DeleteBorrower(existingBorrower);
            await _unitOfWork.CompleteAsync();
        }

        #endregion


        #region Business Logic Methods

        // Business Logic Methods
        public async Task<IEnumerable<BorrowingHistoryDto>> GetBorrowingHistoryAsync(int borrowerId)
        {
            var borrower = await _unitOfWork.BorrowerRepository.GetBorrowerByIdIncludeBorrowingsAndBookAsync(borrowerId,false);

            ThrowNotFoundExceptionIfBorrowerNotExist(borrowerId, borrower);

            var borrowingHistory = _mapper.Map<List<BorrowingHistoryDto>>(borrower.Borrowings);

            return borrowingHistory;    
        }

        public async Task<IEnumerable<BookSummaryDto>> GetCurrentLoansAsync(int borrowerId)
        {
            var borrower = await _unitOfWork.BorrowerRepository.GetBorrowerByIdIncludeBorrowingsAndBookAsync(borrowerId, false);

            if (borrower == null) throw new NotFoundException($"Borrower with id {borrowerId} not found.");

            var currentLoans = _mapper.Map<List<BookSummaryDto>>(borrower.Borrowings
                                                                    .Where(borrowing => !borrowing.IsReturned));

            return currentLoans;
        }

        public async Task<IEnumerable<OverdueLoanDto>> GetOverdueLoansAsync(int borrowerId)
        {
            var borrower = await _unitOfWork.BorrowerRepository
                            .GetBorrowerByIdIncludeBorrowingsAndBookAsync(borrowerId,false);

            ThrowNotFoundExceptionIfBorrowerNotExist(borrowerId, borrower);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var overDueThreshold = today.AddDays(-_loanSettings.DueDays);

            var overDueLoans = _mapper.Map<List<OverdueLoanDto>>(borrower.Borrowings
                                                  .Where( borrowing => !borrowing.IsReturned 
                                                  && borrowing.BorrowDate < overDueThreshold ));


            foreach(var overDueLoan in overDueLoans)
            {
                overDueLoan.DueDate = overDueLoan.BorrowDate.AddDays(_loanSettings.DueDays);
                overDueLoan.OverDueDays =
                    today.DayNumber - overDueLoan.DueDate.DayNumber;
            }

            return overDueLoans;
        }

        public async Task<PagedListDto<BorrowerSummaryDto>> SearchBorrowersByNameAsync(SearchParameters searchParameters)
        {

            var borrowersWithMetaDataList = await _unitOfWork.BorrowerRepository.SearchBorrowersByNameAsync(searchParameters, false);

            var borrowerDtos = _mapper.Map<IEnumerable<BorrowerSummaryDto>>(borrowersWithMetaDataList);

            return new PagedListDto<BorrowerSummaryDto>
            { Items = borrowerDtos, MetaData = borrowersWithMetaDataList.MetaData };
        }

        #endregion



        #region Helper 

        private static readonly Func<IQueryable<Borrower>, IIncludableQueryable<Borrower, Person>> _includePersonExpression =
         query => query.Include(borrower => borrower.Person);

        

        private static readonly Func<IQueryable<Borrower>, IIncludableQueryable<Borrower, Person>> _includeBorrowingsAndPersonExpression =
         query => query.Include(borrower => borrower.Borrowings).Include(borrower => borrower.Person);



        private void ThrowNotFoundExceptionIfBorrowerNotExist(int id, Borrower? borrower)
        {
            if (borrower == null)
                throw new NotFoundException($"Borrower with id {id} not found.");
        }


        #endregion
    }
}
