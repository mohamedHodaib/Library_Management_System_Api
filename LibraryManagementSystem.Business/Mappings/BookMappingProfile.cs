using AutoMapper;
using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.DataAccess.Entities;

namespace LibraryManagementSystem.Business.Mappings
{
    public class BookMappingProfile : Profile
    {
        public BookMappingProfile()
        {
            //For displaying a summary list of Books
            CreateMap<Book, BookSummaryDto>();

            //For displaying a Current Loans
            CreateMap<Borrowing, BookSummaryDto>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Book.Id))
                  .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title));

            //For displaying Book Details
            CreateMap<Book, BookDetailsDto>()
                  .ForMember(dest => dest.AuthorNames,
                            opt => opt.MapFrom(src => src.Authors.Select(a => a.Person.Name.ToString())));

            // For creating a Book from a DTO
            CreateMap<CreateBookDto, Book>()
                  .ForMember(dest => dest.Id ,opt => opt.Ignore())
                  .ForMember(dest => dest.Authors, opt => opt.Ignore())
                  .ForMember(dest => dest.Borrowers, opt => opt.Ignore())
                  .ForMember(dest => dest.Borrowings, opt => opt.Ignore());

            //For Update a Book
            CreateMap<UpdateBookDto, Book>().ReverseMap();

        }
    }
}
