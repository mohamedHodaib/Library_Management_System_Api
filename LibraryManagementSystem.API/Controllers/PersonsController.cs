using LibraryManagementSystem.API.Constants;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.PersonDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.API.Controllers
{
    /// <summary>
    /// API endpoints for managing generic person profiles. All endpoints require Admin privileges.
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController] -- we adding this attribute in the assembly level in the program.c
    [Authorize(Roles = Roles.Admin)]
    [Tags("Persons")] // Groups these endpoints under "Persons" in Swagger UI
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;
        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Gets a specific person by their unique ID.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the person.</param>
        /// <returns>The requested person.</returns>
        /// <response code="200">Returns the requested person.</response>
        /// <response code="404">If a person with the specified ID is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet("{id}", Name = "GetPersonById")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(PersonDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById([Range(1, int.MaxValue)] int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            return Ok(person);
        }

        /// <summary>
        /// Gets a paginated list of all persons.
        /// </summary>
        /// <param name="pagination">The parameters for pagination (page number and page size).</param>
        /// <returns>A paginated list of persons.</returns>
        /// <response code="200">Returns the list of persons for the current page, with pagination metadata in the 'X-Pagination' header.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(IEnumerable<PersonDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters pagination)
        {
            var pagedResult = await _personService.GetAllPersonsAsync(pagination);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves a collection of persons by their IDs.
        /// </summary>
        /// <remarks>
        /// This endpoint uses a POST request to avoid long URLs when fetching many persons.
        /// </remarks>
        /// <param name="getByIdsDto">A list of person IDs to retrieve.</param>
        /// <returns>A list of persons matching the provided IDs.</returns>
        /// <response code="200">Returns a list of the requested persons.</response>
        /// <response code="400">If the request body is null or invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("GetByIds")]
        [ProducesResponseType(typeof(IEnumerable<PersonSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByIds([FromBody] GetByIdsDto getByIdsDto)
        {
            var persons = await _personService.GetPersonsByIdsAsync(getByIdsDto.Ids);
            return Ok(persons);
        }

        /// <summary>
        /// Creates a new person profile.
        /// </summary>
        /// <param name="dto">The data for the new person.</param>
        /// <returns>The newly created person profile.</returns>
        /// <response code="201">Returns the newly created person and a location header pointing to it.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
        {
            var newPerson = await _personService.CreatePersonAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newPerson.Id }, newPerson);
        }

        /// <summary>
        /// Creates a collection of persons.
        /// </summary>
        /// <param name="createCollectionDto">A list of persons to create.</param>
        /// <returns>The details of the created persons.</returns>
        /// <response code="201">Returns the collection of created persons.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("Collection")]
        [ProducesResponseType(typeof(IEnumerable<PersonDetailsDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionDto<CreatePersonDto> createCollectionDto)
        {
            var peopleDetails = await _personService.CreatePersonsCollectionAsync(createCollectionDto.CreateDtos);
            return StatusCode(201, peopleDetails);
        }

        /// <summary>
        /// Updates an existing person's profile.
        /// </summary>
        /// <param name="id" example="1">The ID of the person to update.</param>
        /// <param name="dto">The updated data for the person.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the person was updated successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="404">If the person is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([Range(1, int.MaxValue)] int id, [FromBody] UpdatePersonDto dto)
        {
            await _personService.UpdateAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a person's profile.
        /// </summary>
        /// <param name="id" example="1">The ID of the person to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the person was deleted successfully.</response>
        /// <response code="404">If the person is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([Range(1, int.MaxValue)] int id)
        {
            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
    }
}