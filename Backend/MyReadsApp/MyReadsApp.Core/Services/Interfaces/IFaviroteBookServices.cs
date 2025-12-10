using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FaviorateBook;
using MyReadsApp.Core.Entities;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IFaviroteBookServices 
    {
        Task<Response<FaviorateBookResponse>> CreateAsync(FaviorateBook entity);
        Task<Response<FaviorateBookResponse>> DeleteAsync(Guid BookId);

        Task<Response<FaviorateBookResponse>> GetFavBookAsync(Guid BookId);
    }
}
