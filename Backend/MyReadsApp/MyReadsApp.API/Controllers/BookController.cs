using MyReadsApp.API.DTOs.Book.BookRequest;
using MyReadsApp.API.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles book management endpoints including retrieving, creating, updating, deleting, rating, and searching books.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        private readonly IBookServices _bookServices;


        public BookController(IBookServices BookServices)
        {
            _bookServices = BookServices;
        }

        /// <summary>
        /// Retrieves a specific book by its identifier.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book.</param>
        /// <returns>
        /// HTTP response containing book details or not found error.
        /// </returns>
        [HttpGet("{BookId}")]
        public async Task<IActionResult> GetBook(Guid BookId)
        {
            var result = await _bookServices.GetAsync(BookId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="request">Book creation data including title, description, image, author, and content.</param>
        /// <returns>
        /// HTTP response indicating success or failure of book creation.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreatedBookRequest request)
        {
            // Map API DTO to Core DTO
            var coreRequest = new MyReadsApp.Core.DTOs.Book.Request.CreateBookRequest(
                request.BookImage,
                request.Description,
                request.Title,
                request.Content,
                request.AuthorId
            );

            // Call core service with Core DTO
            var result = await _bookServices.CreateAsync(coreRequest);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            return CreatedAtAction(
                actionName: "GetBook",
                routeValues: new { BookId = result.Value.Id },
                value: result.Value);
        }

        /// <summary>
        /// Updates an existing book by its identifier.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book to update.</param>
        /// <param name="request">Book update data.</param>
        /// <returns>
        /// HTTP response indicating success or failure of the update.
        /// </returns>
        [HttpPut("{BookId}")]
        public async Task<IActionResult> UpdateBook(Guid BookId, UpdateBookRequest request)
        {
            // Map API DTO to Core DTO
            var coreUpdate = new MyReadsApp.Core.DTOs.Book.Request.UpdateBookRequest(
                request.BookImage,
                request.Description,
                request.Title,
                request.AuthorId
            );

            // Call core service update with Core DTO
            var result = await _bookServices.UpdateAsync(BookId, coreUpdate);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            return NoContent();
        }

        /// <summary>
        /// Deletes a book by its identifier.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book to delete.</param>
        /// <returns>
        /// HTTP response indicating success or failure of deletion.
        /// </returns>
        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteBook(Guid BookId)
        {
            var result = await _bookServices.DeleteAsync(BookId);
            if (!result.IsSuccess) 
                return StatusCode(result.StatusCode, result);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a paginated list of books, optionally filtered by category.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination (default: 1).</param>
        /// <param name="pageSize">The number of books per page (default: 10).</param>
        /// <param name="categoryId">Optional category identifier to filter books.</param>
        /// <returns>
        /// HTTP response containing paginated collection of books.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? categoryId = null)
        {
            var res = await _bookServices.GetListAsync(pageNumber, pageSize, categoryId);
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Searches for books by query string, optionally filtered by category.
        /// </summary>
        /// <param name="q">The search query string.</param>
        /// <param name="pageNumber">The page number for pagination (default: 1).</param>
        /// <param name="pageSize">The number of results per page (default: 10).</param>
        /// <param name="categoryId">Optional category identifier to filter search results.</param>
        /// <returns>
        /// HTTP response containing paginated collection of search results.
        /// </returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? categoryId = null)
        {
            var res = await _bookServices.SearchAsync(q, pageNumber, pageSize, categoryId);
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Rates a book by the authenticated user.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book to rate.</param>
        /// <param name="request">Rating data including the rating value.</param>
        /// <returns>
        /// HTTP response indicating success or failure of the rating.
        /// </returns>
        [HttpPost("{bookId}/rating")]
        [Authorize]
        public async Task<IActionResult> Rate(Guid bookId, [FromBody] MyReadsApp.Core.DTOs.Book.Request.RateBookRequest request)
        {
            var sub = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(sub)) return Forbid();
            var userId = Guid.Parse(sub);
            var res = await _bookServices.RateBookAsync(bookId, userId, request.Value);
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Retrieves the rating summary for a book, optionally including the current user's rating.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// HTTP response containing rating summary for the book.
        /// </returns>
        [HttpGet("{bookId}/rating")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRating(Guid bookId)
        {
            Guid? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var sub = User.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(sub)) userId = Guid.Parse(sub);
            }
            var res = await _bookServices.GetRatingSummaryAsync(bookId, userId);
            return StatusCode(res.StatusCode, res);
        }
    }
}
