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
                return BadRequest(result);
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
                return BadRequest(result);

            return NoContent();
        }

        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteBook(Guid BookId)
        {
            var result = await _bookServices.DeleteAsync(BookId);
            if (!result.IsSuccess) 
                return BadRequest(result);
            return NoContent();
        }
    }
}
