using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.AccountDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementSystem.API.Controllers
{
    /// <summary>
    /// Manages user accounts, including registration and authentication.
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController] -- we adding this attribute in the assembly level in the program.cs
    [Tags("Auth")] // Groups these endpoints under "Books" in Swagger UI
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService accountService)
        {
            _authenticationService = accountService;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="registerDto">The data required to register a new user.</param>
        /// <returns>A 201 Created status code if registration is successful.</returns>
        /// <response code="201">Indicates the user was created successfully.</response>
        /// <response code="400">If the registration data is invalid (e.g., duplicate email, password does not meet requirements).</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var (identityResult, userProfileDto) = await _authenticationService.RegisterAsync(registerDto);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(UsersController.GetById),
                                    "Users" , new { id = userProfileDto!.Id }, userProfileDto);
        }

        /// <summary>
        /// Authenticates a user and provides access and refresh tokens.
        /// </summary>
        /// <param name="loginDto">The user's login credentials.</param>
        /// <returns>An object containing the JWT access token and refresh token.</returns>
        /// <response code="200">Returns the access and refresh tokens.</response>
        /// <response code="401">If the credentials are incorrect.</response>
        /// <response code="400">If the Login data are not valid form.</response>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authenticationService.ValidateAsync(loginDto);

            if (!result) return Unauthorized("Wrong Username or Password.");

            var tokenDto = await _authenticationService.GenerateTokens(true);

            return Ok(tokenDto);
        }

        /// <summary>
        /// Authenticates a user and provides access and refresh tokens.
        /// </summary>
        /// <param name="logoutDto">The user's refresh token.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the user is logged out successfully.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="400">If the refresh token is incorrect.</response>
        [HttpPut("Logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logoutDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                Unauthorized("User Id claim is not provided in the token.");

            //If revocation failed ,it doesn't matter because the client code will direct to login page.
            await _authenticationService.RevokeRefreshToken(logoutDto.RefreshToken, userId!);

            return NoContent();
        }


        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="tokenDto">An object containing the expired access token and the valid refresh token.</param>
        /// <returns>A new set of access and refresh tokens.</returns>
        /// <response code="200">Returns a new set of tokens.</response>
        /// <response code="400">If the provided tokens are invalid or the refresh token has expired.</response>
        [HttpPost("Refresh")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            var tokenDtoToReturn = await _authenticationService.RefreshToken(tokenDto);

            return Ok(tokenDtoToReturn);
        }

        #region Password End Points 

        /// <summary>
        /// Initiates a password reset by sending a reset link to the user's email.
        /// </summary>
        /// <param name="forgotPasswordDto">The email address of the user requesting password reset.</param>
        /// <returns>No content on successful email dispatch.</returns>
        /// <response code="204">Password reset email has been sent successfully.</response>
        /// <response code="400">If the email address is invalid.</response>
        [HttpPut("ForgotPassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            await _authenticationService.ForgotPasswordAsync(forgotPasswordDto);
            return NoContent();
        }

        /// <summary>
        /// Resets a user's password using a valid reset token.
        /// </summary>
        /// <param name="resetPasswordDto">Contains the email, reset token, new password, and password confirmation.</param>
        /// <returns>No content on successful password reset.</returns>
        /// <response code="204">Password has been reset successfully.</response>
        /// <response code="400">If the reset token is invalid, expired, or the new password doesn't meet requirements.</response>
        [HttpPut("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var identityResult = await _authenticationService.ResetPasswordAsync(resetPasswordDto);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Changes the authenticated user's password.
        /// </summary>
        /// <param name="changePasswordDto">Contains the current password, new password, and password confirmation.</param>
        /// <returns>No content on successful password change.</returns>
        /// <response code="204">Password has been changed successfully.</response>
        /// <response code="400">If the current password is incorrect or the new password doesn't meet requirements.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPut("ChangePassword")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID claim is not provided in the token.");

            var identityResult = await _authenticationService.ChangePasswordAsync(userId, changePasswordDto);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        #endregion
    }
}