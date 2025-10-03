using AutoMapper;
using LibraryManagementSystem.Business.Dtos.AccountDtos;
using LibraryManagementSystem.Business.Dtos.PersonDtos;
using LibraryManagementSystem.Business.Dtos.UserDtos;
using LibraryManagementSystem.DataAccess.Entities.People;

namespace LibraryManagementSystem.Business.Mappings
{
    public class PersonMappingProfile : Profile
    {
        public PersonMappingProfile()
        {
            //for displaying persondetailsdto from person
            CreateMap<Person, PersonDetailsDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
                    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Not assigned"));

            //for creating or updating person from CreateAndUpdatePerson Dto
            CreateMap<ManipulatePersonDto, Person>()
                    .ForMember(dest => dest.Id,opt => opt.Ignore())
                    .ForPath(dest => dest.Name.FirstName,
                             opt => opt.MapFrom(src => src.FirstName))
                    .ForPath(dest => dest.Name.LastName,
                             opt => opt.MapFrom(src => src.LastName));

            //For displaying personsummarydto
            CreateMap<Person, PersonSummaryDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null));


            //For Create person from CreateUserByAdminDto
            CreateMap<CreateUserByAdminDto, Person>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForPath(dest => dest.Name.FirstName,opt => opt.MapFrom(src => src.FirstName))
                    .ForPath(dest => dest.Name.LastName,opt => opt.MapFrom(src => src.LastName));

            //For Create person from CreateUserByAdminDto
            CreateMap<UpdateUserProfileDto, UpdatePersonDto>();

            // For creating a Person from a RegisterDTO
            CreateMap<RegisterDto, Person>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.Author, opt => opt.Ignore())
                  .ForMember(dest => dest.Borrower, opt => opt.Ignore())
                  .ForMember(dest => dest.User, opt => opt.Ignore())
                  .ForPath(dest => dest.Name.FirstName,
                             opt => opt.MapFrom(src => src.FirstName))
                  .ForPath(dest => dest.Name.LastName,
                             opt => opt.MapFrom(src => src.LastName));
        }

    }
}
