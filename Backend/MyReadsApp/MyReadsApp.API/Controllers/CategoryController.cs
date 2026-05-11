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
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyReadsApp.Core.Services.Interfaces.ICategoryServices _categoryServices;

        public CategoryController(MyReadsApp.Core.Services.Interfaces.ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var res = await _categoryServices.GetAllAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var res = await _categoryServices.CreateAsync(request);
            if (!res.IsSuccess)
                return StatusCode(res.StatusCode, res);
            return CreatedAtAction(nameof(Get), new { id = res.Value.Id }, res);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var res = await _categoryServices.UpdateAsync(id, request);
            return StatusCode(res.StatusCode, res);
        }
    }
}
