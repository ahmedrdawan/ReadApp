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
    /// <summary>
    /// Handles favorite book management endpoints including creating, deleting, and retrieving favorite books.
    /// </summary>
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

        /// <summary>
        /// Adds a book to the user's favorites.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book to favorite.</param>
        /// <returns>
        /// HTTP response indicating success or failure of adding to favorites.
        /// </returns>
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
            if(!result.IsSuccess)
                return BadRequest(result);
            return 
                CreatedAtAction(
                    actionName: "GetFaviorateBook",
                    routeValues : new { BookId = fb.BookId  },
                    value: result.Value);
        }

        /// <summary>
        /// Removes a book from the user's favorites.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book to remove from favorites.</param>
        /// <returns>
        /// HTTP response indicating success or failure of removing from favorites.
        /// </returns>
        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteFaviorateBook(Guid BookId)
        {
            var result = await _faviroteBookServices.DeleteAsync(BookId);
            if(!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a favorite book by its identifier.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book.</param>
        /// <returns>
        /// HTTP response containing favorite book details or not found error.
        /// </returns>
        [HttpGet("{BookId}")]
        public async Task<IActionResult> GetFaviorateBook(Guid BookId)
        {
            var result = await _faviroteBookServices.GetFavBookAsync(BookId);
            if(!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
    }
}
