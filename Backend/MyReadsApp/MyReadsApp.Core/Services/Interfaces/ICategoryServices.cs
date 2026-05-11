using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Category.Request;
using MyReadsApp.Core.DTOs.Category.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface ICategoryServices
    {
        Task<Response<IEnumerable<CategoryResponse>>> GetAllAsync();
        Task<Response<CategoryResponse>> CreateAsync(CreateCategoryRequest request);
        Task<Response<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request);
    }
}
