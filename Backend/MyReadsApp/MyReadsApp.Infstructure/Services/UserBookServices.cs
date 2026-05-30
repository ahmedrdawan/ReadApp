using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.UserBook.Response;
using MyReadsApp.Core.DTOs.UserBook.Request;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Infrastructure implementation for user book (shelf) operations. Handles persistence and caching for user book entries.
    /// </summary>
    public class UserBookServices : IUserBookServices
    {
        private readonly IGenericRepository<UserBook> _repository;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public UserBookServices(IGenericRepository<UserBook> repository, AppDbContext context, IDistributedCache cache)
        {
            _repository = repository;
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Creates a new user book entry after validating uniqueness.
        /// </summary>
        /// <param name="request">Create user book request DTO.</param>
        /// <returns>A Response containing the created user book response.</returns>
        public async Task<Response<UserBookResponse>> CreateAsync(CreateUserBookRequest request)
        {
            var userBookExisting = await _context.UserBooks
                .FirstOrDefaultAsync(us => us.UserId == request.UserId && us.BookId == request.BookId);

            if (userBookExisting != null)
                return Response<UserBookResponse>.Failure("The userBook Is already Exist", 409);

            var entity = new UserBook
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                BookId = request.BookId,
                Statuts = request.Statuts,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(entity);
            await _cache.RemoveAsync(GetAllCacheKey());
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);
            return Response<UserBookResponse>.Success(response);
        }

        /// <summary>
        /// Deletes a user book entry by its identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the user book to delete.</param>
        /// <returns>A Response containing the deleted user book response.</returns>
        public async Task<Response<UserBookResponse>> DeleteAsync(Guid Id)
        {
            var userBookExisting = await _context.UserBooks.FindAsync(Id);
            if (userBookExisting == null)
                return Response<UserBookResponse>.Failure("The userBook Is Not Found", 404);

            await _repository.DeleteAsync(userBookExisting);
            await _cache.RemoveAsync(GetCacheKey(Id));
            await _cache.RemoveAsync(GetAllCacheKey());
            return Response<UserBookResponse>.Success(BuildResponse(userBookExisting));
        }

        /// <summary>
        /// Retrieves all user books, using cache when available.
        /// </summary>
        /// <returns>A Response containing all user book responses or failure if none found.</returns>
        public Response<IEnumerable<UserBookResponse>> GetAllAsync()
        {
            var cached = _cache.GetRecordAsync<List<UserBookResponse>>(GetAllCacheKey()).GetAwaiter().GetResult();
            if (cached is not null)
                return cached.Count == 0
                    ? Response<IEnumerable<UserBookResponse>>.Failure("The userBook Is Not Found", 404)
                    : Response<IEnumerable<UserBookResponse>>.Success(cached);

            List<UserBookResponse> userBooks = _context.UserBooks
                .Select(us => BuildResponse(us))
                .ToList();

            _cache.SetRecordAsync(GetAllCacheKey(), userBooks).GetAwaiter().GetResult();

            if (userBooks.Count == 0)
                return Response<IEnumerable<UserBookResponse>>.Failure("The userBook Is Not Found", 404);

            return Response<IEnumerable<UserBookResponse>>.Success(userBooks);
        }

        /// <summary>
        /// Retrieves a user book by its identifier, using cache when available.
        /// </summary>
        /// <param name="Id">The unique identifier of the user book.</param>
        /// <returns>A Response containing the user book response.</returns>
        public async Task<Response<UserBookResponse>> GetByIdAsync(Guid Id)
        {
            var key = GetCacheKey(Id);
            var cached = await _cache.GetRecordAsync<UserBookResponse>(key);
            if (cached is not null)
                return Response<UserBookResponse>.Success(cached);

            var userBookExisting = await _context.UserBooks.FindAsync(Id);
            if (userBookExisting == null)
                return Response<UserBookResponse>.Failure("The userBook Is Not Found", 404);

            var response = BuildResponse(userBookExisting);
            await _cache.SetRecordAsync(key, response);
            return Response<UserBookResponse>.Success(response);
        }

        /// <summary>
        /// Updates an existing user book entry with the provided status.
        /// </summary>
        /// <param name="Id">The unique identifier of the user book to update.</param>
        /// <param name="request">Update user book request DTO.</param>
        /// <returns>A Response containing the updated user book response.</returns>
        public async Task<Response<UserBookResponse>> UpdateAsync(Guid Id, UpdateUserBookRequest request)
        {
            var userBookExisting = await _context.UserBooks.FindAsync(Id);
            if (userBookExisting == null)
                return Response<UserBookResponse>.Failure("The userBook Is Not Found", 404);

            userBookExisting.Statuts = request.Statuts;
            await _repository.UpdateAsync(userBookExisting);

            var response = BuildResponse(userBookExisting);
            await _cache.SetRecordAsync(GetCacheKey(Id), response);
            await _cache.RemoveAsync(GetAllCacheKey());
            return Response<UserBookResponse>.Success(response);
        }

        private static string GetCacheKey(Guid id) => $"UserBook:{id}";
        private static string GetAllCacheKey() => "UserBook:All";

        private static UserBookResponse BuildResponse(UserBook entity)
        {
            return new UserBookResponse
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Statuts = entity.Statuts,
            };
        }
    }
}
