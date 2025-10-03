using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.Business.Dtos.BorrowerDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

namespace LibraryManagementSystem.API.Controllers
{
    /// <summary>
    /// API endpoints for managing library borrowers. All endpoints require Admin privileges.
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController] -- we adding this attribute in the assembly level in the program.c
    [Authorize(Roles = "Admin")]
    [Tags("Borrowers")] // Groups these endpoints under "Borrowers" in Swagger UI
    public class BorrowersController : ControllerBase
    {
        private readonly IBorrowerService _borrowerService;
        public BorrowersController(IBorrowerService borrowerService)
        {
            _borrowerService = borrowerService;
        }

        /// <summary>
        /// Gets a specific borrower by their unique ID.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the borrower.</param>
        /// <returns>The requested borrower.</returns>
        /// <response code="200">Returns the requested borrower.</response>
        /// <response code="404">If a borrower with the specified ID is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet("{id}", Name = "GetBorrowerById")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(BorrowerDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById([Range(1, int.MaxValue)] int id)
        {
            var borrower = await _borrowerService.GetBorrowerByIdAsync(id);
            return Ok(borrower);
        }

        /// <summary>
        /// Gets a paginated list of all borrowers.
        /// </summary>
        /// <param name="pagination">The parameters for pagination (page number and page size).</param>
        /// <returns>A paginated list of borrowers.</returns>
        /// <response code="200">Returns the list of borrowers for the current page, with pagination metadata in the 'X-Pagination' header.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(IEnumerable<BorrowerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters pagination)
        {
            var pagedResult = await _borrowerService.GetAllBorrowersAsync(pagination);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves a collection of borrowers by their IDs.
        /// </summary>
        /// <remarks>
        /// This endpoint uses a POST request to avoid long URLs when fetching many borrowers.
        /// </remarks>
        /// <param name="getByIdsDto">A list of borrower IDs to retrieve.</param>
        /// <returns>A list of borrowers matching the provided IDs.</returns>
        /// <response code="200">Returns a list of the requested borrowers.</response>
        /// <response code="400">If the request body is null or invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("GetByIds")]
        [ProducesResponseType(typeof(IEnumerable<BorrowerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByIds([FromBody] GetByIdsDto getByIdsDto)
        {
            var borrowers = await _borrowerService.GetBorrowersByIdsAsync(getByIdsDto.Ids);
            return Ok(borrowers);
        }

        /// <summary>
        /// Gets the complete borrowing history for a specific borrower.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the borrower.</param>
        /// <returns>The borrower's full loan history.</returns>
        /// <response code="200">Returns the borrowing history.</response>
        /// <response code="404">If the borrower is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet("{id}/BorrowingHistory")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(BorrowingHistoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetBorrowingHistory([Range(1, int.MaxValue)] int id)
        {
            var borrowingHistory = await _borrowerService.GetBorrowingHistoryAsync(id);
            return Ok(borrowingHistory);
        }

        /// <summary>
        /// Gets a list of books currently on loan to a specific borrower.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the borrower.</param>
        /// <returns>A list of the borrower's current loans.</returns>
        /// <response code="200">Returns the list of current loans.</response>
        /// <response code="404">If the borrower is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet("{id}/Loans/Current")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(IEnumerable<BookSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCurrentLoans([Range(1, int.MaxValue)] int id)
        {
            var currentLoans = await _borrowerService.GetCurrentLoansAsync(id);
            return Ok(currentLoans);
        }

        /// <summary>
        /// Gets a list of overdue books for a specific borrower.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the borrower.</param>
        /// <returns>A list of overdue books.</returns>
        /// <response code="200">Returns the list of overdue loans.</response>
        /// <response code="404">If the borrower is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet("{id}/Loans/Overdue")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(IEnumerable<OverdueLoanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetOverdueLoans([Range(1, int.MaxValue)] int id)
        {
            var overdueBooks = await _borrowerService.GetOverdueLoansAsync(id);
            return Ok(overdueBooks);
        }

        /// <summary>
        /// Searches for borrowers by name (paginated).
        /// </summary>
        /// <param name="searchParameters">The search term and pagination parameters.</param>
        /// <returns>A paginated list of borrowers that match the search term.</returns>
        /// <response code="200">Returns a paginated list of matching borrowers.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpGet("Search")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(PagedResult<BorrowerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SearchBorrowersByName([FromQuery] SearchParameters searchParameters)
        {
            var pagedResult = await _borrowerService.SearchBorrowersByNameAsync(searchParameters);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Creates a new borrower profile.
        /// </summary>
        /// <param name="dto">The data for the new borrower.</param>
        /// <returns>The newly created borrower profile.</returns>
        /// <response code="201">Returns the newly created borrower and a location header pointing to it.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BorrowerDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateBorrowerDto dto)
        {
            var newBorrower = await _borrowerService.CreateBorrowerAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newBorrower.Id }, newBorrower);
        }

        /// <summary>
        /// Creates a collection of borrowers.
        /// </summary>
        /// <param name="createCollectionDto">A list of borrowers to create.</param>
        /// <returns>The details of the created borrowers.</returns>
        /// <response code="201">Returns the collection of created borrowers.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("Collection")]
        [ProducesResponseType(typeof(IEnumerable<BorrowerDetailsDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionDto<CreateBorrowerDto> createCollectionDto)
        {
            var personsDetails = await _borrowerService.CreateBorrowerCollectionAsync(createCollectionDto.CreateDtos);
            return StatusCode(201, personsDetails);
        }

        /// <summary>
        /// Deletes a borrower profile.
        /// </summary>
        /// <param name="id" example="1">The ID of the borrower to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the borrower was deleted successfully.</response>
        /// <response code="404">If the borrower is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([Range(1, int.MaxValue)] int id)
        {
            await _borrowerService.DeleteBorrowerAsync(id);
            return NoContent();
        }
    }
}