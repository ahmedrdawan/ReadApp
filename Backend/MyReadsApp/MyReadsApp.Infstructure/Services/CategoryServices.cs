using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Category.Request;
using MyReadsApp.Core.DTOs.Category.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Infrastructure service for category CRUD operations. Ensures uniqueness, persistence,
    /// and provides responses suitable for API consumption.
    /// </summary>
    public class CategoryServices : ICategoryServices
    {
        private readonly AppDbContext _context;

        public CategoryServices(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new category after validating uniqueness of the category name.
        /// </summary>
        /// <param name="request">Create category request DTO.</param>
        /// <returns>A Response containing the created category response.</returns>
        public async Task<Response<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Name))
                return Response<CategoryResponse>.Failure("Name is required", 400);

            var exists = await _context.Categories.AnyAsync(c => c.Name == request.Name);
            if (exists)
                return Response<CategoryResponse>.Failure("Category already exists", 409);

            var cat = new Category { Id = Guid.NewGuid(), Name = request.Name, Icon = request.Icon };
            await _context.Categories.AddAsync(cat);
            await _context.SaveChangesAsync();

            var resp = new CategoryResponse { Id = cat.Id, Name = cat.Name, Icon = cat.Icon };
            return Response<CategoryResponse>.Success(resp);
        }

        /// <summary>
        /// Retrieves all categories, sorted alphabetically by name.
        /// </summary>
        /// <returns>A Response containing all category responses.</returns>
        public async Task<Response<IEnumerable<CategoryResponse>>> GetAllAsync()
        {
            var items = await _context.Categories.OrderBy(c => c.Name)
                .Select(c => new CategoryResponse { Id = c.Id, Name = c.Name, Icon = c.Icon })
                .ToListAsync();

            return Response<IEnumerable<CategoryResponse>>.Success(items);
        }

        /// <summary>
        /// Updates an existing category with the provided name and icon.
        /// </summary>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <param name="request">Update category request DTO.</param>
        /// <returns>A Response containing the updated category response.</returns>
        public async Task<Response<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null)
                return Response<CategoryResponse>.Failure("Category not found", 404);

            if (!string.IsNullOrWhiteSpace(request?.Name))
                cat.Name = request.Name;
            cat.Icon = request.Icon;

            _context.Categories.Update(cat);
            await _context.SaveChangesAsync();

            var resp = new CategoryResponse { Id = cat.Id, Name = cat.Name, Icon = cat.Icon };
            return Response<CategoryResponse>.Success(resp);
        }
    }
}
