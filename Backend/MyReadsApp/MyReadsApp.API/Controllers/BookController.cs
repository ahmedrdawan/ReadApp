using MyReadsApp.API.DTOs.Book.BookRequest;
using MyReadsApp.API.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MyReadsApp.API.Controllers
{
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
            return result is null ? NotFound() : Ok(result);
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
            return result <= 0
                ? BadRequest(request)
                : CreatedAtAction(
                    actionName: "GetBook",
                    routeValues: new { BookId = book.Id },
                    value: new BookAuthorResponse
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Description = book.Description,
                        BookImage = book.BookImage,
                        AuthorId = book.AuthorId,
                    });
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
            return result <= 0 ? BadRequest(BookId) : NoContent();
        }

        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteBook(Guid BookId)
        {
            var result = await _bookServices.DeleteAsync(BookId);
            return result <= 0 ? BadRequest(BookId) : NoContent();
        }
    }
}
