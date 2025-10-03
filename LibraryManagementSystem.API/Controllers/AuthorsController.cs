using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.AuthorDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

namespace LibraryManagementSystem.API.Controllers
{
    /// <summary>
    /// API endpoints for managing authors.
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController] -- we adding this attribute in the assembly level in the program.cs
    [Tags("Authors")] // Groups these endpoints under "Authors" in Swagger UI
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        #region Public read endpoints

        /// <summary>
        /// Gets a specific author by their unique ID.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the author.</param>
        /// <returns>The requested author.</returns>
        /// <response code="200">Returns the requested author.</response>
        /// <response code="404">If an author with the specified ID is not found.</response>
        [HttpGet("{id}", Name = "GetAuthorById")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(AuthorDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([Range(1, int.MaxValue)] int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            return Ok(author);
        }

        /// <summary>
        /// Gets a paginated list of all authors.
        /// </summary>
        /// <param name="pagination">The parameters for pagination (page number and page size).</param>
        /// <returns>A paginated list of authors.</returns>
        /// <response code="200">Returns the paginated list of authors with pagination metadata in the response headers.</response>
        [HttpGet]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(PagedResult<AuthorSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters pagination)
        {
            var pagedResult = await _authorService.GetAllAuthorsAsync(pagination);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves a collection of authors by their IDs.
        /// </summary>
        /// <remarks>
        /// This endpoint uses a POST request to avoid long URLs when fetching many authors.
        /// </remarks>
        /// <param name="getByIdsDto">A list of author IDs to retrieve.</param>
        /// <returns>A list of authors matching the provided IDs.</returns>
        /// <response code="200">Returns a list of the requested authors.</response>
        /// <response code="400">If the request body is null or invalid.</response>
        [HttpPost("GetByIds")]
        [ProducesResponseType(typeof(IEnumerable<AuthorSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByIds([FromBody] GetByIdsDto getByIdsDto)
        {
            var authors = await _authorService.GetAuthorsByIdsAsync(getByIdsDto.Ids);
            return Ok(authors);
        }

        /// <summary>
        /// Searches for authors by name (paginated).
        /// </summary>
        /// <param name="searchParameters">The search term and pagination parameters.</param>
        /// <returns>A paginated list of authors that match the search term.</returns>
        /// <response code="200">Returns a paginated list of matching authors.</response>
        [HttpGet("Search")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 60)]
        [ProducesResponseType(typeof(PagedResult<AuthorSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchAuthorsByName([FromQuery] SearchParameters searchParameters)
        {
            var pagedResult = await _authorService.SearchAuthorsByNameAsync(searchParameters);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Gets statistics for a specific author.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the author.</param>
        /// <returns>Statistics for the author (e.g., number of books).</returns>
        /// <response code="200">Returns the author's statistics.</response>
        /// <response code="404">If the author is not found.</response>
        [HttpGet("{id}/Stats")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(AuthorStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthorStatistics([Range(1, int.MaxValue)] int id)
        {
            var auhtorStats = await _authorService.GetAuthorStatisticsAsync(id);
            return Ok(auhtorStats);
        }

        #endregion

        #region Admin endpoints

        /// <summary>
        /// Creates a new author. (Admin role required)
        /// </summary>
        /// <param name="dto">The data for the new author.</param>
        /// <returns>The newly created author.</returns>
        /// <response code="201">Returns the newly created author and a location header pointing to it.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AuthorDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto)
        {
            var newAuthor = await _authorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newAuthor.Id }, newAuthor);
        }

        /// <summary>
        /// Creates a collection of authors. (Admin role required)
        /// </summary>
        /// <param name="createCollectionDto">A list of authors to create.</param>
        /// <returns>The details of the created authors.</returns>
        /// <response code="201">Returns the collection of created authors.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("Collection")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<AuthorDetailsDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionDto<CreateAuthorDto> createCollectionDto)
        {
            var authorsDetails = await _authorService.CreateCollectionAsync(createCollectionDto.CreateDtos);
            return StatusCode(201, authorsDetails);
        }

        /// <summary>
        /// Updates an existing author. (Admin role required)
        /// </summary>
        /// <param name="id" example="1">The ID of the author to update.</param>
        /// <param name="dto">The updated data for the author.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the author was updated successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="404">If the author is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([Range(1, int.MaxValue)] int id, [FromBody] UpdateAuthorDto dto)
        {
            await _authorService.UpdateAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Deletes an author. (Admin role required)
        /// </summary>
        /// <param name="id" example="1">The ID of the author to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the author was deleted successfully.</response>
        /// <response code="404">If the author is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([Range(1, int.MaxValue)] int id)
        {
            await _authorService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}