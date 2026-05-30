using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using MyReadsApp.Core.DTOs.Category.Response;
using MyReadsApp.Core.DTOs.Category.Request;
using MyReadsApp.Core.Common;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles category management endpoints including retrieving, creating, and updating book categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyReadsApp.Core.Services.Interfaces.ICategoryServices _categoryServices;

        public CategoryController(MyReadsApp.Core.Services.Interfaces.ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        /// <summary>
        /// Retrieves all available categories.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of all categories.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var res = await _categoryServices.GetAllAsync();
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Creates a new book category.
        /// </summary>
        /// <param name="request">Category creation data including name and description.</param>
        /// <returns>
        /// HTTP response indicating success or failure of category creation.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var res = await _categoryServices.CreateAsync(request);
            if (!res.IsSuccess)
                return StatusCode(res.StatusCode, res);
            return CreatedAtAction(nameof(Get), new { id = res.Value.Id }, res);
        }

        /// <summary>
        /// Updates an existing category by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <param name="request">Category update data.</param>
        /// <returns>
        /// HTTP response indicating success or failure of the update.
        /// </returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var res = await _categoryServices.UpdateAsync(id, request);
            return StatusCode(res.StatusCode, res);
        }
    }
}
