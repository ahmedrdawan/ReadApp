using MyReadsApp.Core.DTOs.FaviorateBook;
using MyReadsApp.Core.Entities;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IFaviroteBookServices 
    {
        Task<int> CreateAsync(FaviorateBook entity);
        Task<int> DeleteAsync(Guid BookId);

        Task<FaviorateBookResponse?> GetFavBookAsync(Guid BookId);
    }
}
