using LibraryManagementSystem.Business.Dtos.AccountDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Dtos.UserDtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Contract
{

        public interface IUserService
        {
            Task<UserProfileDto> GetUserProfileAsync(string userId);
            Task<IEnumerable<UserSummaryDto>> GetAllUsersAsync(PaginationParameters paginationParameters);
            Task<(IdentityResult,UserProfileDto?)> CreateUserAsync(CreateUserByAdminDto dto);
            Task<IdentityResult> UpdateUserProfileAsync(string userId, UpdateUserProfileDto dto);
            Task<IdentityResult> DeleteUserAsync(string userId);
            Task<IdentityResult> AssignRoleAsync(AssignRoleDto dto);
            Task<IdentityResult> RemoveRoleFromUserAsync(AssignRoleDto dto);
        }
}
