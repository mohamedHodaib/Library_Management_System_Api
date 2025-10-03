using AutoMapper;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.AuthorDtos;
using LibraryManagementSystem.Business.Dtos.PersonDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Dtos.UserDtos;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Services
{
    public sealed class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IPersonService _personService;

        public UserService(UserManager<User> userManager
            , IMapper mapper,RoleManager<IdentityRole> roleManager
            ,IPersonService personService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _personService = personService;
        }


        #region CRUD Methods

        public async Task<UserProfileDto> GetUserProfileAsync(string id)
        {
            var user = await _userManager.Users
                            .Include(u => u.Person)
                            .FirstOrDefaultAsync(user => user.Id == id);

            ThrowNotFoundExceptionIfUserNotExist(id, user);

            var userDto = _mapper.Map<UserProfileDto>(user);

            userDto.Roles = await _userManager.GetRolesAsync(user);

            return userDto;
        }

        public async Task<IEnumerable<UserSummaryDto>> GetAllUsersAsync(PaginationParameters paginationParameters)
        {
            var users = await _userManager.Users.Include(u => u.Person)
                                                 .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                                                 .Take(paginationParameters.PageSize)
                                                 .ToListAsync();

            return _mapper.Map<List<UserSummaryDto>>(users);
        }

        public async Task<(IdentityResult,UserProfileDto?)> CreateUserAsync(CreateUserByAdminDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Email is already in use." }),null);
            }

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return (IdentityResult.Failed(new IdentityError { Description = "Role not found." }),null);

            var person = _mapper.Map<Person>(dto);

            var user = new User { UserName = dto.Email, Email =  dto.Email ,Person = person};

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            var userProfileDto = _mapper.Map<UserProfileDto>(user);

            userProfileDto.Roles = await _userManager.GetRolesAsync(user);

            return (result,userProfileDto);
        }


        public async Task<IdentityResult> UpdateUserProfileAsync(string id, UpdateUserProfileDto userToUpdateDto)
        {
            var user = await _userManager.FindByIdAsync(id);

            ThrowNotFoundExceptionIfUserNotExist(id, user);

            //Update Person
            await _personService.UpdateAsync(user.PersonId,_mapper.Map<UpdatePersonDto>(userToUpdateDto));

            //Update User
            await _userManager.SetEmailAsync(user,userToUpdateDto.Email);
            await _userManager.SetPhoneNumberAsync(user,userToUpdateDto.Phone);

            var result = await _userManager.UpdateAsync(user);

            return result;
        }

       
        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            ThrowNotFoundExceptionIfUserNotExist(id, existingUser);

            var result = await _userManager.DeleteAsync(existingUser!);
            await _personService.DeletePersonAsync(existingUser!.PersonId);

            return result;
        }

        #endregion

        #region Business logic  
        public async Task<IdentityResult> AssignRoleAsync(AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);

            ThrowNotFoundExceptionIfUserNotExist(assignRoleDto.UserId, user);

            if (!await _roleManager.RoleExistsAsync(assignRoleDto.RoleName))
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            if (await _userManager.IsInRoleAsync(user, assignRoleDto.RoleName))
                return IdentityResult.Failed(new IdentityError { Description = "User already has this role." });

            return await _userManager.AddToRoleAsync(user, assignRoleDto.RoleName);

        }

        public async Task<IdentityResult> RemoveRoleFromUserAsync(AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);

            ThrowNotFoundExceptionIfUserNotExist(assignRoleDto.UserId, user);

            if (!await _roleManager.RoleExistsAsync(assignRoleDto.RoleName))
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

            if (!await _userManager.IsInRoleAsync(user, assignRoleDto.RoleName))
                return IdentityResult.Failed(new IdentityError { Description = "User don't have this role." });

            return await _userManager.RemoveFromRoleAsync(user, assignRoleDto.RoleName);
        }


        private void ThrowNotFoundExceptionIfUserNotExist(string id, User? user)
        {
            if (user == null)
                throw new NotFoundException($"User with id {id} not found.");
        }
        #endregion

    }
}
