using AutoMapper;
using LibraryManagementSystem.Business.Dtos.UserDtos;
using LibraryManagementSystem.DataAccess.Entities.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            //For displaying user Details
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person.Name.ToString()))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            //For dispalying user Summary
            CreateMap<User, UserSummaryDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person.Name.ToString()));


        }

    }
}
