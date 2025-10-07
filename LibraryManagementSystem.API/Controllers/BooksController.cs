using LibraryManagementSystem.API.Constants;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Dtos.BookDtos;
using LibraryManagementSystem.Business.Dtos.Shared;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

namespace LibraryManagementSystem.API.Controllers
{
    /// <summary>
    /// API endpoints for managing books in the library.
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController] -- we adding this attribute in the assembly level in the program.c
    [Tags("Books")] // Groups these endpoints under "Books" in Swagger UI
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        #region Public read endpoints

        /// <summary>
        /// Gets a specific book by its unique ID.
        /// </summary>
        /// <param name="id" example="1">The unique identifier of the book.</param>
        /// <returns>The requested book.</returns>
        /// <response code="200">Returns the requested book.</response>
        /// <response code="404">If a book with the specified ID is not found.</response>
        [HttpGet("{id}", Name = "GetBookById")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(BookDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([Range(1, int.MaxValue)] int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(book);
        }

        /// <summary>
        /// Gets a paginated list of all books.
        /// </summary>
        /// <param name="pagination">The parameters for pagination (page number and page size).</param>
        /// <returns>A paginated list of books.</returns>
        /// <response code="200">Returns the paginated list of books with pagination metadata in the response headers.</response>
        [HttpGet]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(PagedResult<BookSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters pagination)
        {
            var pagedResult = await _bookService.GetAllBooksAsync(pagination);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves a collection of books by their IDs.
        /// </summary>
        /// <remarks>
        /// This endpoint uses a POST request to avoid long URLs when fetching many books.
        /// </remarks>
        /// <param name="getByIdsDto">A list of book IDs to retrieve.</param>
        /// <returns>A list of books matching the provided IDs.</returns>
        /// <response code="200">Returns a list of the requested books.</response>
        /// <response code="400">If the request body is null or invalid.</response>
        [HttpPost("GetByIds")]
        [ProducesResponseType(typeof(IEnumerable<BookSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByIds([FromBody] GetByIdsDto getByIdsDto)
        {
            var books = await _bookService.GetBooksByIdsAsync(getByIdsDto.Ids);
            return Ok(books);
        }

        /// <summary>
        /// Gets a book by its International Standard Book Number (ISBN).
        /// </summary>
        /// <param name="isbn" example="978-0321765723">The ISBN of the book.</param>
        /// <returns>The requested book.</returns>
        /// <response code="200">Returns the requested book.</response>
        /// <response code="404">If a book with the specified ISBN is not found.</response>
        [HttpGet("isbn/{isbn}")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(BookDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIsbn([StringLength(13, MinimumLength = 10)] string isbn)
        {
            var book = await _bookService.GetByIsbnAsync(isbn);
            return Ok(book);
        }

        /// <summary>
        /// Gets all books written by a specific author.
        /// </summary>
        /// <param name="authorId" example="1">The unique ID of the author.</param>
        /// <returns>A list of books by the specified author.</returns>
        /// <response code="200">Returns a list of books.</response>
        /// <response code="404">If the author ID does not exist.</response>
        [HttpGet("by-author/{authorId}")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(IEnumerable<BookSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBooksByAuthorIdAsync([Range(1, int.MaxValue)] int authorId)
        {
            var books = await _bookService.GetBooksByAuthorIdAsync(authorId);
            return Ok(books);
        }

        /// <summary>
        /// Gets a list of all available books (not currently borrowed).
        /// </summary>
        /// <returns>A list of available books.</returns>
        /// <response code="200">Returns a list of available books.</response>
        [HttpGet("available")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ProducesResponseType(typeof(IEnumerable<BookSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableBooksAsync()
        {
            var books = await _bookService.GetAvailableBooksAsync();
            return Ok(books);
        }

        /// <summary>
        /// Searches for books by author name or title (paginated).
        /// </summary>
        /// <param name="searchParameters">The search term and pagination parameters.</param>
        /// <returns>A paginated list of books that match the search term.</returns>
        /// <response code="200">Returns a paginated list of matching books.</response>
        [HttpGet("Search")]
        [HttpCacheExpiration]
        [HttpCacheValidation]
        [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 60)]
        [ProducesResponseType(typeof(PagedResult<BookSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchBooksByNameOrTitleAsync([FromQuery] SearchParameters searchParameters)
        {
            var pagedResult = await _bookService.SearchBooksByAuthorNameOrTitleAsync(searchParameters);
            return Ok(pagedResult);
        }
        #endregion

        #region Admin endpoints

        /// <summary>
        /// Creates a new book. (Admin role required)
        /// </summary>
        /// <param name="createBookDto">The data for the new book.</param>
        /// <returns>The newly created book.</returns>
        /// <response code="201">Returns the newly created book and a location header pointing to it.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(BookDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateBookDto createBookDto)
        {
            var newBook = await _bookService.CreateBookAsync(createBookDto);
            return CreatedAtAction(nameof(GetById), new { id = newBook.Id }, newBook);
        }

        /// <summary>
        /// Creates a collection of books. (Admin role required)
        /// </summary>
        /// <param name="createCollectionDto">A list of books to create.</param>
        /// <returns>The details of the created books.</returns>
        /// <response code="201">Returns the collection of created books.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("Collection")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(IEnumerable<BookDetailsDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionDto<CreateBookDto> createCollectionDto)
        {
            var bookDetails = await _bookService.CreateBooksCollectionAsync(createCollectionDto.CreateDtos);
            return StatusCode(201, bookDetails);
        }

        /// <summary>
        /// Updates an existing book. (Admin role required)
        /// </summary>
        /// <param name="id" example="1">The ID of the book to update.</param>
        /// <param name="updateBookDto">The updated data for the book.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the book was updated successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="404">If the book is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([Range(1, int.MaxValue)] int id, [FromBody] UpdateBookDto updateBookDto)
        {
            await _bookService.UpdateAsync(id, updateBookDto);

            return NoContent();
        }

        /// <summary>
        /// Partially updates a book using a JSON Patch document. (Admin role required)
        /// </summary>
        /// <remarks>
        /// A JSON Patch document allows for atomic updates to a resource.
        /// Example: [ { "op": "replace", "path": "/title", "value": "New Title" } ]
        /// </remarks>
        /// <param name="id" example="1">The ID of the book to patch.</param>
        /// <param name="patchDoc">The JSON Patch document with update operations.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the book was patched successfully.</response>
        /// <response code="400">If the patch document is invalid or the resulting model fails validation.</response>
        /// <response code="404">If the book is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPatch("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Patch([Range(1, int.MaxValue)] int id, [FromBody] JsonPatchDocument<UpdateBookDto> patchDoc)
        {
            var dtoToPatch = await _bookService.GetUpdateBookDto(id);

            patchDoc.ApplyTo(dtoToPatch, ModelState);

            if (!TryValidateModel(dtoToPatch)) return ValidationProblem(ModelState);

            await _bookService.UpdateAsync(id, dtoToPatch);

            return NoContent();
        }

        /// <summary>
        /// Assigns an author to a book. (Admin role required)
        /// </summary>
        /// <param name="bookId" example="1">The ID of the book.</param>
        /// <param name="authorId" example="1">The ID of the author to assign.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the author was added successfully.</response>
        /// <response code="404">If the book or author is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpPost("{bookId}/Authors/{authorId}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(BookDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddAuthorToBookAsync([Range(1, int.MaxValue)] int bookId, [Range(1, int.MaxValue)] int authorId)
        {
            var bookDetailsDto = await _bookService.AddAuthorToBookAsync(bookId, authorId);

            if (bookDetailsDto == null)
                return BadRequest("This author already exist in this book's authors.");

            return CreatedAtAction(nameof(GetById), new { id = bookId }, bookDetailsDto);
        }

        /// <summary>
        /// Deletes a book. (Admin role required)
        /// </summary>
        /// <param name="id" example="1">The ID of the book to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Indicates the book was deleted successfully.</response>
        /// <response code="404">If the book is not found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the 'Admin' role.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([Range(1, int.MaxValue)] int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }

        #endregion

        #region Admin or borrower endpoints

        /// <summary>
        /// Borrows a book for the authenticated user. (Admin or Borrower role required)
        /// </summary>
        /// <param name="bookId" example="1">The ID of the book to borrow.</param>
        /// <returns>A success message.</returns>
        /// <response code="400">If the book is not available to be borrowed.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an Admin or Borrower.</response>
        /// <response code="404">If the book is not found.</response>
        [HttpPost("{bookId}/Borrow")]
        [Authorize(Roles = Roles.Borrower)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BorrowBookAsync([Range(1, int.MaxValue)] int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User Id claim is not provided in the token.");

            var borrowingDetailsDto = await _bookService.BorrowBookAsync(bookId, userId);
            return StatusCode(201, borrowingDetailsDto);
        }

        /// <summary>
        /// Returns a borrowed book. (Admin or Borrower role required)
        /// </summary>
        /// <param name="bookId" example="1">The ID of the book to return.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the book was returned successfully.</response>
        /// <response code="400">If the book was not currently borrowed.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an Admin or Borrower.</response>
        /// <response code="404">If the book is not found.</response>
        [HttpPut("{bookId}/Return")]
        [Authorize(Roles = Roles.Borrower)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnBookAsync([Range(1, int.MaxValue)] int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User Id claim is not provided in the token.");

            await _bookService.ReturnBookAsync(bookId, userId);
            return NoContent();
        }

        #endregion
    }
}