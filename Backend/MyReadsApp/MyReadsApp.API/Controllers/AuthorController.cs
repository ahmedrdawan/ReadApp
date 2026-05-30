using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.AuthorRequest;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles author management endpoints including retrieving, creating, updating, and deleting authors.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorServices _authorServices;

        public AuthorController(IAuthorServices authorServices)
        {
            _authorServices = authorServices;
        }

        /// <summary>
        /// Retrieves an author by their identifier.
        /// </summary>
        /// <param name="AuthorId">The unique identifier of the author.</param>
        /// <returns>
        /// HTTP response containing author details or not found error.
        /// </returns>
        [HttpGet("{AuthorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthor(Guid AuthorId)
        {
            var result = await _authorServices.GetAsync(AuthorId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Value);
        }

        /// <summary>
        /// Creates a new author.
        /// </summary>
        /// <param name="request">Author creation data including name, image, and biography.</param>
        /// <returns>
        /// HTTP response indicating success or failure of author creation.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] CreatedAuthorRequest request)
        {
            // Map API DTO to Core DTO
            var coreRequest = new MyReadsApp.Core.DTOs.Author.CreateAuthorRequest(
                request.AuthorName ?? string.Empty,
                request.AuthorImage,
                request.Bio
            );

            var result = await _authorServices.CreateAsync(coreRequest);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(
                    actionName: "GetAuthor",
                    routeValues: new { AuthorId = result.Value.Id },
                    value: result.Value
                );
        }

        /// <summary>
        /// Updates an existing author by their identifier.
        /// </summary>
        /// <param name="AuthorId">The unique identifier of the author to update.</param>
        /// <param name="request">Author update data including name, image, and biography.</param>
        /// <returns>
        /// HTTP response indicating success or failure of the update.
        /// </returns>
        [HttpPut("{AuthorId}")]
        public async Task<IActionResult> UpdateAuthor(Guid AuthorId, UpdatedAuthorRequest request)
        {
            var coreUpdate = new MyReadsApp.Core.DTOs.Author.UpdateAuthorRequest(
                request.AuthorName,
                request.AuthorImage,
                request.Bio
            );

            var result = await _authorServices.UpdateAsync(AuthorId, coreUpdate);
            if (!result.IsSuccess)
                return BadRequest(result);

            return NoContent();
        }

        /// <summary>
        /// Deletes an author by their identifier.
        /// </summary>
        /// <param name="AuthorId">The unique identifier of the author to delete.</param>
        /// <returns>
        /// HTTP response indicating success or failure of deletion.
        /// </returns>
        [HttpDelete("{AuthorId}")]
        public async Task<IActionResult> DeleteAuthor(Guid AuthorId)
        {
            var result = await _authorServices.DeleteAsync(AuthorId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return NoContent();
        }
    }
}
