using LibraryManagementSystem.Business.Dtos.AccountDtos;
using LibraryManagementSystem.Business.Dtos.UserDtos;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Business.Contract
{
    public interface IAuthenticationService
    {
        Task<(IdentityResult,UserProfileDto?)> RegisterAsync(RegisterDto registerDto);
        Task<bool> ValidateAsync(LoginDto dto);
        Task<TokenDto> GenerateTokens(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> RevokeRefreshToken(string token,string userId);
    }
}
