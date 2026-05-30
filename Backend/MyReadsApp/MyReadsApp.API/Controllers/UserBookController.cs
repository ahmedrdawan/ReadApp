using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.UserBook.request;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles user book management endpoints including creating, updating, deleting, and retrieving user books.
    /// </summary>
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

        /// <summary>
        /// Creates a new user book record for the current user.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book.</param>
        /// <param name="request">User book creation data including status.</param>
        /// <returns>
        /// HTTP response indicating success or failure of user book creation.
        /// </returns>
        [HttpPost("{BookId}")]
        public async Task<IActionResult> CreateUserBook(Guid BookId, [FromBody] CreateUserBookRequest request)
        {
            // Map API DTO to Core DTO (include current user and book)
            var coreRequest = new MyReadsApp.Core.DTOs.UserBook.CreateUserBookRequest(BookId, _userAuthServices.GetCurrentUser(), request.Statuts);

            var result = await _UserBookServices.CreateAsync(coreRequest);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(
                    actionName: "GetUserBook",
                    routeValues: new { UserBookId = result.Value.Id },
                    value: result.Value);
        }

        /// <summary>
        /// Updates an existing user book record.
        /// </summary>
        /// <param name="UserBookId">The unique identifier of the user book to update.</param>
        /// <param name="request">User book update data including status.</param>
        /// <returns>
        /// HTTP response indicating success or failure of the update.
        /// </returns>
        [HttpPut("{UserBookId}")]
        public async Task<IActionResult> UpdateUserBook(Guid UserBookId, [FromBody] UpdateUserBookRequest request)
        {
            // Map API DTO to Core DTO
            var coreUpdate = new MyReadsApp.Core.DTOs.UserBook.UpdateUserBookRequest(request.Statuts);

            var result = await _UserBookServices.UpdateAsync(UserBookId, coreUpdate);
            if (!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

        /// <summary>
        /// Deletes a user book record by its identifier.
        /// </summary>
        /// <param name="UserBookId">The unique identifier of the user book to delete.</param>
        /// <returns>
        /// HTTP response indicating success or failure of deletion.
        /// </returns>
        [HttpDelete("{UserBookId}")]
        public async Task<IActionResult> DeleteUserBook(Guid UserBookId)
        {
            var result = await _UserBookServices.DeleteAsync(UserBookId);
            if (!result.IsSuccess)
                return NotFound(result);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a specific user book by its identifier.
        /// </summary>
        /// <param name="UserBookId">The unique identifier of the user book.</param>
        /// <returns>
        /// HTTP response containing user book details or not found error.
        /// </returns>
        [HttpGet("{UserBookId}")]
        public async Task<IActionResult> GetUserBook(Guid UserBookId)
        {
            var result = await _UserBookServices.GetByIdAsync(UserBookId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves all user books for the current user.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of user books.
        /// </returns>
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

