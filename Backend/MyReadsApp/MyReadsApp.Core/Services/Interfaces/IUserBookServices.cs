using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.UserBook.Response;
using MyReadsApp.Core.DTOs.UserBook.Request;
using MyReadsApp.Core.Entities;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IUserBookServices
    {
        Task<Response<UserBookResponse>> CreateAsync(CreateUserBookRequest request);
        Task<Response<UserBookResponse>> DeleteAsync(Guid Id);
        Task<Response<UserBookResponse>> UpdateAsync(Guid Id, UpdateUserBookRequest request);
        Task<Response<UserBookResponse>> GetByIdAsync(Guid Id);
        Response<IEnumerable<UserBookResponse>> GetAllAsync();
    }
}
