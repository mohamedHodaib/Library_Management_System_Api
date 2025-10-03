using LibraryManagementSystem.Business.Dtos.BorrowerDtos;
using LibraryManagementSystem.DataAccess.Entities.People;
using AutoMapper;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.Business.Dtos.BookDtos;


namespace LibraryManagementSystem.Business.Mappings
{
    internal class BorrowerMappingProfile : Profile
    {
        public BorrowerMappingProfile()
        {

            //For displaying a summary list of Borrowers
            CreateMap<Borrower, BorrowerSummaryDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person.Name.ToString()));

            //For displaying Borrower Details
            CreateMap<Borrower, BorrowerDetailsDto>()
                .ForMember(dest => dest.CurrentLoansCount,
                            opt => opt.MapFrom(src => src.Borrowings.Count(b => !b.IsReturned)))
                .ForMember(dest => dest.Name,
                             opt => opt.MapFrom(src => src.Person.Name.ToString()));

            //for displaying a borrowing History 
            CreateMap<Borrowing, BorrowingHistoryDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title));

            //for displaying a OverDue Loans 
            CreateMap<Borrowing, OverdueLoanDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.DueDate, opt => opt.Ignore())
                .ForMember(dest => dest.OverDueDays, opt =>  opt.Ignore());


            // For creating a Borrower from a CreateAndUpdateBorrowerDto
            CreateMap<CreateBorrowerDto, Borrower>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.Books, opt => opt.Ignore())
                  .ForMember(dest => dest.Borrowings, opt => opt.Ignore())
                  .ForMember(dest => dest.Person, opt => opt.Ignore());


        }

    }
}
