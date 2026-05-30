using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FaviorateBook;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Infrastructure implementation for favorite book operations. Handles persistence and caching of favorite books.
    /// </summary>
    public class FaviroteBookServices : IFaviroteBookServices
    {
        private readonly IGenericRepository<FaviorateBook> _repository;
        private readonly AppDbContext _context;
        private readonly IUserAuthServices _userAuthServices;
        private readonly IDistributedCache _cache;

        public FaviroteBookServices(IGenericRepository<FaviorateBook> repository, AppDbContext context, IUserAuthServices userAuthServices, IDistributedCache cache)
        {
            _repository = repository;
            _context = context;
            _userAuthServices = userAuthServices;
            _cache = cache;
        }

        /// <summary>
        /// Creates a new favorite book entry for the current user after validating user, book, and uniqueness.
        /// </summary>
        /// <param name="entity">The favorite book entity to create.</param>
        /// <returns>A Response containing the created favorite book response.</returns>
        public async Task<Response<FaviorateBookResponse>> CreateAsync(FaviorateBook entity)
        {
            var user = await _context.Users.FindAsync(entity.UserId);
            if (user == null)
                return Response<FaviorateBookResponse>.Failure("The User Not Found", 404);

            if (user.Id != _userAuthServices.GetCurrentUser())
                return Response<FaviorateBookResponse>.Failure("The User Not Authorized", 403);

            var book = await _context.Books.FindAsync(entity.BookId);
            if (book == null)
                return Response<FaviorateBookResponse>.Failure("The Book Not Found", 404);

            var existing = await _context.FaviorateBooks
                .AnyAsync(f => f.UserId == entity.UserId && f.BookId == entity.BookId);

            if (existing)
                return Response<FaviorateBookResponse>.Failure("The Favorite Book Already Exists", 409);

            await _repository.CreateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.UserId, entity.BookId), response);

            return Response<FaviorateBookResponse>.Success(response);
        }

        private static FaviorateBookResponse BuildResponse(FaviorateBook entity)
        {
            return new FaviorateBookResponse
            {
                UserId = entity.UserId,
                BookId = entity.BookId,
                CreatedAt = entity.CreatedAt,
            };
        }

        /// <summary>
        /// Removes a favorite book from the current user's favorites.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book to remove from favorites.</param>
        /// <returns>A Response containing the deleted favorite book response.</returns>
        public async Task<Response<FaviorateBookResponse>> DeleteAsync(Guid bookId)
        {
            var userId = _userAuthServices.GetCurrentUser();

            var favBook = await _context.FaviorateBooks
                .FirstOrDefaultAsync(fb => fb.UserId == userId && fb.BookId == bookId);

            if (favBook == null)
                return Response<FaviorateBookResponse>.Failure("The Favorite Book Not Found", 404);

            _context.FaviorateBooks.Remove(favBook);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync(GetCacheKey(userId, bookId));

            return Response<FaviorateBookResponse>.Success(BuildResponse(favBook));
        }

        /// <summary>
        /// Retrieves a favorite book entry for the current user and specified book, using cache when available.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>A Response containing the favorite book response.</returns>
        public async Task<Response<FaviorateBookResponse>> GetFavBookAsync(Guid bookId)
        {
            var userId = _userAuthServices.GetCurrentUser();
            var key = GetCacheKey(userId, bookId);

            var cached = await _cache.GetRecordAsync<FaviorateBookResponse>(key);
            if (cached is not null)
                return Response<FaviorateBookResponse>.Success(cached);

            var favBook = await _context.FaviorateBooks
                .SingleOrDefaultAsync(fb => fb.UserId == userId && fb.BookId == bookId);

            if (favBook == null)
                return Response<FaviorateBookResponse>.Failure("Favorite Book Not Found", 404);

            var response = BuildResponse(favBook);
            await _cache.SetRecordAsync(key, response);
            return Response<FaviorateBookResponse>.Success(response);
        }

        private static string GetCacheKey(Guid userId, Guid bookId) => $"FavoriteBook:{userId}:{bookId}";
    }
}
