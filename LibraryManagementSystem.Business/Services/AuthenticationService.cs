using AutoMapper;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.AccountDtos;
using LibraryManagementSystem.Business.Dtos.UserDtos;
using LibraryManagementSystem.Business.Exceptions;
using LibraryManagementSystem.Business.Options;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LibraryManagementSystem.Business.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ForgotPasswordSettings _forgotPasswordSettings;
        private readonly JwtSettings _jwtSettings;

        private User? _user;



        public AuthenticationService(UserManager<User> userManager
            , SignInManager<User> signInManager
            , IMapper mapper
            , ILogger<AuthenticationService> logger
            , IEmailService emailService
            , IOptionsMonitor<ForgotPasswordSettings> forgotPasswordSettingsOptions
            , IOptionsMonitor<JwtSettings> jwtSettingsOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _forgotPasswordSettings = forgotPasswordSettingsOptions.CurrentValue;
            _jwtSettings = jwtSettingsOptions.CurrentValue;
        }

        public async Task<(IdentityResult,UserProfileDto?)> RegisterAsync(RegisterDto registerDto)
        {

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
                throw new ConflictException("An account with this Email Address Already Exists.");

            //Create person and Borrower or Author 
            var person = _mapper.Map<Person>(registerDto);

            switch (registerDto.AccountType)
            {
                case AccountType.Borrower:
                    var borrower = new Borrower();
                    person.Borrower = borrower;
                    break;

                case AccountType.Author:
                    var author = new Author();

                    if (string.IsNullOrEmpty(registerDto.Biography))
                        return (IdentityResult.Failed(
                            new IdentityError 
                            { 
                                Description = "Biography must be provided for Author Account"
                            }
                        ),
                        null);

                    person.Author = author;
                    break;


                default:
                    return (IdentityResult.Failed(
                            new IdentityError 
                            { 
                                Description = "Invalid Account Type"
                            }
                    ),
                    null);
            }


            // create User

            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Person = person
            };

            var identityResult = await _userManager.CreateAsync(user, registerDto.Password);

            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, registerDto.AccountType.ToString());
            }

            var userProfileDto = _mapper.Map<UserProfileDto>(user);

            userProfileDto.Roles = await _userManager.GetRolesAsync(user);

            return (identityResult,userProfileDto);
        }




        public async Task<bool> ValidateAsync(LoginDto dto)
        {

            _user = await _userManager.FindByNameAsync(dto.UserName);

            var result = (_user != null
                         && (await _signInManager.CheckPasswordSignInAsync
                                    (_user, dto.Password, lockoutOnFailure: true)).Succeeded);

            if (!result)
            {
                _logger.LogWarning($"{nameof(ValidateAsync)} Authentication failed.Wrong User name or password");
            }

            return result;
        }


        public async Task<TokenDto> GenerateTokens(bool populateExp)
        {
            //Prepare Access Token

            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();

            //create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = signingCredentials,
            };


            //create and write the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var accessToken = tokenHandler.WriteToken(token);


            //Prepare Refresh Token 
            var refreshToken = GetRefreshToken();

            _user.RefreshToken = refreshToken;

            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpireDays);

            await _userManager.UpdateAsync(_user);

            return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipleFromExpiredToken(tokenDto.AccessToken);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User Id Claim is not exist in the token.");

            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null
                || !user.RefreshToken!.Equals(tokenDto.RefreshToken)
                || user.RefreshTokenExpiryTime <= DateTime.Now
                || user.IsRefreshTokenRevoked)
                throw new UnauthorizedException("Invalid,expired or revoked refresh token.");

            _user = user;

            return await GenerateTokens(false);
        }


        public async Task<bool> RevokeRefreshToken(string token,string id)
        {
            var user = _userManager.Users
                        .FirstOrDefault(user => user.RefreshToken!.Equals(token) && user.Id == id);

            if (user == null) return false;

            user.IsRefreshTokenRevoked = true;
            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // SECURITY: If no user is found, do NOT throw an error.
            //    Simply return without doing anything. This prevents attackers from
            //    guessing which emails are registered in your system (user enumeration).
            if (user == null) return;


            //Generate one-time password reset token 
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //build the password reset link for the email
            var frontendUrl = _forgotPasswordSettings.FrontendUrl;
            var encodedToken = HttpUtility.UrlEncode(token);
            var resetLink = $"{frontendUrl}/reset-password?email={dto.Email}&token={encodedToken}";

            //sending email to the user with the reset link that have the token
            await _emailService.SendPasswordResetEmailAsync(dto.Email, resetLink);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Password Reset Failed." });

            //Reset the password
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Unable to Change Password." });

            //change the password
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            return result;
        }


        #region Helper

        private SigningCredentials GetSigningCredentials()
        {
            var key = _jwtSettings.Key;


            //Create security key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            //Create signing credentials
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            //Create Claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,_user.Id),
                new Claim(JwtRegisteredClaimNames.Name,_user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(_user);

            //Add roles to claims
            var rolesClaims = roles.Select(role => new Claim("role", role));
            claims.AddRange(rolesClaims);

            return claims;
        }


        private string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token,tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256
                            ,StringComparison.InvariantCultureIgnoreCase))
                throw new BadRequestException("Expired token invalid.");

            return principal;
        }



        #endregion


    }
}
