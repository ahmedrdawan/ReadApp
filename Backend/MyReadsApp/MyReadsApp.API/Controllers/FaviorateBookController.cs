using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.DTOs.FaviorateBook;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using System.ComponentModel.Design;

namespace MyReadsApp.API.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/Faviorates")]
    [ApiController]
    public class FaviorateBookController : ControllerBase
    {
        private readonly IUserAuthServices _userAuthServices;
        private readonly IFaviroteBookServices _faviroteBookServices;

        public FaviorateBookController(IUserAuthServices userAuthServices, IFaviroteBookServices faviroteBookServices)
        {
            _userAuthServices = userAuthServices;
            _faviroteBookServices = faviroteBookServices;
        }

        [HttpPost("{BookId}")]
        public async Task<IActionResult> CreateFaviorateBook(Guid BookId)
        {
            var fb = new FaviorateBook
            {
                BookId = BookId,
                UserId = _userAuthServices.GetCurrentUser(),
                CreatedAt = DateTime.UtcNow
            };

            var result = await _faviroteBookServices.CreateAsync(fb);
            return result <= 0 ? BadRequest() : 
                CreatedAtAction(
                    actionName: "GetFaviorateBook",
                    routeValues : new { BookId = fb.BookId  },
                    value: new FaviorateBookResponse
                    {
                        BookId = fb.BookId,
                        UserId = fb.UserId,
                        CreatedAt = fb.CreatedAt
                    });
        }

        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteFaviorateBook(Guid BookId)
        {
            var result = await _faviroteBookServices.DeleteAsync(BookId);
            return result <= 0 ? BadRequest(BookId) : NoContent();
        }

        [HttpGet("{BookId}")]
        public async Task<IActionResult> GetFaviorateBook(Guid BookId)
        {
            var result = await _faviroteBookServices.GetFavBookAsync(BookId);
            return result == null ? NotFound($"Favorite book not found for BookId: {BookId}") : Ok(result);
        }
    }
}
