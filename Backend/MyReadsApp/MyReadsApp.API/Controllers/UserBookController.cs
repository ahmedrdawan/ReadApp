using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.UserBook.request;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.API.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserBookController : ControllerBase
    {
        private readonly IUserAuthServices _userAuthServices;
        private readonly IUserBookServices _UserBookServices;

        public UserBookController(IUserAuthServices userAuthServices, IUserBookServices UserBookServices)
        {
            _userAuthServices = userAuthServices;
            _UserBookServices = UserBookServices;
        }

        [HttpPost("{BookId}")]
        public async Task<IActionResult> CreateUserBook(Guid BookId, [FromBody] CreateUserBookRequest request)
        {
            var us = new UserBook
            {
                Id = Guid.NewGuid(),
                BookId = BookId,
                UserId = _userAuthServices.GetCurrentUser(),
                CreatedAt = DateTime.UtcNow,
                Statuts = request.Statuts
            };

            var result = await _UserBookServices.CreateAsync(us);
            if (!result.IsSuccess)
                return BadRequest(result);
            return
                CreatedAtAction(
                    actionName: "GetUserBook",
                    routeValues: new { UserBookId = us.Id },
                    value: result.Value);
        }
        [HttpPut("{UserBookId}")]
        public async Task<IActionResult> UpdateUserBook(Guid UserBookId, [FromBody] UpdateUserBookRequest request)
        {
            var newUserBook = new UserBook
            {
                Statuts = request.Statuts,
            };

            var result = await _UserBookServices.UpdateAsync(UserBookId, newUserBook);
            if (!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

        [HttpDelete("{UserBookId}")]
        public async Task<IActionResult> DeleteUserBook(Guid UserBookId)
        {
            var result = await _UserBookServices.DeleteAsync(UserBookId);
            if (!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

        [HttpGet("{UserBookId}")]
        public async Task<IActionResult> GetUserBook(Guid UserBookId)
        {
            var result = await _UserBookServices.GetByIdAsync(UserBookId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }
        [HttpGet]
        public IActionResult GetAllUserBook()
        {
            var result =  _UserBookServices.GetAllAsync();
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }
    }
}

