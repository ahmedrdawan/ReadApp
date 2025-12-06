using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IBookServices : IGenericRepository<Book>
    {
        Task<BookAuthorResponse?> GetAsync(Guid BookId);
    }
}
