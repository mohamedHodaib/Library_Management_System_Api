using AutoMapper;
using LibraryManagementSystem.Business.Dtos.AuthorDtos;
using LibraryManagementSystem.Business.Dtos.BorrowerDtos;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;

namespace LibraryManagementSystem.Business.Mappings
{
    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile() 
        {
            //For displaying a summary list of Authors
            CreateMap<Author, AuthorSummaryDto>()
                .ForMember(dest => dest.Name ,opt => opt.MapFrom(src => src.Person.Name.ToString()));

            //For displaying Borrower Details
            CreateMap<Author, AuthorDetailsDto>()
                .ForMember(dest => dest.BooksCount,
                                opt => opt.MapFrom(src => src.Books.Count))
                .ForMember(dest => dest.Name,
                             opt => opt.MapFrom(src => src.Person.Name.ToString()));


            // For creating a Borrower from a DTO
            CreateMap<CreateAuthorDto, Author>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.Books, opt => opt.Ignore())
                  .ForMember(dest => dest.Person,opt => opt.Ignore());


            // For Updating a Borrower from a DTO
            CreateMap<UpdateAuthorDto, Author>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.Books, opt => opt.Ignore())
                  .ForMember(dest => dest.Person, opt => opt.Ignore())
                  .ForMember(dest => dest.PersonId,opt => opt.Ignore());

            //For Get AuthorStats 
            CreateMap<Author, AuthorStatsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person.Name.ToString()))
                .ForMember(dest => dest.TotalBooksWritten, opt => opt.MapFrom(src => src.Books.Count))
                .ForMember(dest => dest.TotalTimesBorrowed,
                            opt => opt.MapFrom(src => src.Books.SelectMany(book => book.Borrowings).Count()))
                .ForMember(dest => dest.MostPopularBookTitle,
                            opt => opt.MapFrom(src => src.Books.OrderByDescending(book => book.Borrowings.Count())
                            .Select(book => book.Title).FirstOrDefault()));
        }


    }
}
