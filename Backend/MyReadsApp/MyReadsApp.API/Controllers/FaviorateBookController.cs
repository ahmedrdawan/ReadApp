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
            if(!result.IsSuccess)
                return BadRequest(result);
            return 
                CreatedAtAction(
                    actionName: "GetFaviorateBook",
                    routeValues : new { BookId = fb.BookId  },
                    value: result.Value);
        }

        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteFaviorateBook(Guid BookId)
        {
            var result = await _faviroteBookServices.DeleteAsync(BookId);
            if(!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

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
