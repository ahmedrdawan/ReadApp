using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Author.Response;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IAuthorServices
    {
        Task<Response<AuthorResponse>> CreateAsync(Author Author);
        Task<Response<AuthorResponse>> DeleteAsync(Guid AuthorId);
        Task<Response<AuthorResponse>> UpdateAsync(Guid AuthorId, Author newAuthor);
        Task<Response<AuthorResponse>> GetAsync(Guid AuthorId);
    }
}
