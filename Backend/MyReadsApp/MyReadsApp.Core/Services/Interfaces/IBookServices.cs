using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IBookServices
    {
        Task<Response<BookAuthorResponse>> CreateAsync(Book book);
        Task<Response<BookAuthorResponse>> DeleteAsync(Guid BookId);
        Task<Response<BookAuthorResponse>> UpdateAsync(Guid id, Book newBook);
        Task<Response<BookAuthorResponse>> GetAsync(Guid BookId);
    }
}
