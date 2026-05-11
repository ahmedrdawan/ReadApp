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
        Task<Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>> GetListAsync(int pageNumber = 1, int pageSize = 10, Guid? categoryId = null);
        Task<Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>> SearchAsync(string query, int pageNumber = 1, int pageSize = 10, Guid? categoryId = null);
        Task<Response<object>> RateBookAsync(Guid bookId, Guid userId, int value);
        Task<Response<object>> GetRatingSummaryAsync(Guid bookId, Guid? userId = null);
    }
}
