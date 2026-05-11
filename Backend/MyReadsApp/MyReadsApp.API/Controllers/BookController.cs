using MyReadsApp.API.DTOs.Book.BookRequest;
using MyReadsApp.API.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MyReadsApp.API.Controllers
{
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

        [HttpGet("{BookId}")]
        public async Task<IActionResult> GetBook(Guid BookId)
        {
            var result = await _bookServices.GetAsync(BookId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreatedBookRequest request)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                BookImage = request.BookImage,
                Content = request.Content,
                Description = request.Description, 
                AuthorId = request.AuthorId,
                Title = request.Title,
            };

            var result = await _bookServices.CreateAsync(book);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);
            return
                CreatedAtAction(
                    actionName: "GetBook",
                    routeValues: new { BookId = book.Id },
                    value: result.Value);
        }

        [HttpPut("{BookId}")]
        public async Task<IActionResult> UpdateBook(Guid BookId, UpdateBookRequest request)
        {
            var NewBook = new Book
            {
                BookImage = request.BookImage,
                Description = request.Description,
                Title = request.Title,
                AuthorId= request.AuthorId,
            };

            var result = await _bookServices.UpdateAsync(BookId, NewBook);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            return NoContent();
        }

        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteBook(Guid BookId)
        {
            var result = await _bookServices.DeleteAsync(BookId);
            if (!result.IsSuccess) 
                return StatusCode(result.StatusCode, result);
            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? categoryId = null)
        {
            var res = await _bookServices.GetListAsync(pageNumber, pageSize, categoryId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? categoryId = null)
        {
            var res = await _bookServices.SearchAsync(q, pageNumber, pageSize, categoryId);
            return StatusCode(res.StatusCode, res);
        }

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
