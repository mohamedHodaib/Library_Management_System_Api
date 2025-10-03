using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.Business.Dtos.UserDtos;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace LibraryManagementSystem.API.Controllers
{
    /// <summary>
    /// API endpoints for managing user accounts. All endpoints require Admin privileges.
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController] -- we adding this attribute in the assembly level in the program.c
    [Authorize(Roles = "Admin")]
    [Tags("Users")] // Groups these endpoints under "Users" in Swagger UI
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a specific user's profile by their unique ID.
        /// </summary>
        /// <param name="id" example="8e445865-a24d-4543-a6c6-9443d048cdb9">The unique identifier (GUID) of the user.</param>
        /// <returns>The requested user's profile.</returns>
        /// <response code="200">Returns the requested user's profile.</response>
        /// <response code="404">If a user with the specified ID is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet("{id}", Name = "GetUserById")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Gets a paginated list of all users.
        /// </summary>
        /// <param name="pagination">The parameters for pagination (page number and page size).</param>
        /// <returns>A paginated list of users.</returns>
        /// <response code="200">Returns the paginated list of users.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(PagedResult<UserSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters pagination)
        {
            var users = await _userService.GetAllUsersAsync(pagination);
            return Ok(users);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="dto">The data for the new user, including username, email, password, and roles.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">Returns a success message if the user was created.</response>
        /// <response code="400">If the provided data is invalid (e.g., duplicate username, password too weak).</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateUserByAdminDto dto)
        {
            var (identityResult, userProfileDto) = await _userService.CreateUserAsync(dto);

            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult.Errors);
            }

            return StatusCode(201, userProfileDto);
        }

        /// <summary>
        /// Updates an existing user's profile.
        /// </summary>
        /// <param name="id" example="8e445865-a24d-4543-a6c6-9443d048cdb9">The ID of the user to update.</param>
        /// <param name="dto">The updated data for the user's profile.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the user profile was updated successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserProfileDto dto)
        {
            var result = await _userService.UpdateUserProfileAsync(id, dto);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id" example="8e445865-a24d-4543-a6c6-9443d048cdb9">The ID of the user to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the user was deleted successfully.</response>
        /// <response code="400">If the deletion fails (e.g., cannot delete the last admin).</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        /// <summary>
        /// Assigns one or more roles to a user.
        /// </summary>
        /// <param name="assignRoleDto">The user ID and a list of roles to assign.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">Returns a success message if the roles were assigned.</response>
        /// <response code="400">If the operation fails (e.g., user or role not found).</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("Roles/Assign")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var result = await _userService.AssignRoleAsync(assignRoleDto);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Role Assigned Successfully.");
        }

        /// <summary>
        /// Removes one or more roles from a user.
        /// </summary>
        /// <param name="assignRoleDto">The user ID and a list of roles to remove.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">Returns a success message if the roles were removed.</response>
        /// <response code="400">If the operation fails (e.g., user or role not found).</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("Roles/Remove")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var result = await _userService.RemoveRoleFromUserAsync(assignRoleDto);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Role Removed Successfully.");
        }
    }
}