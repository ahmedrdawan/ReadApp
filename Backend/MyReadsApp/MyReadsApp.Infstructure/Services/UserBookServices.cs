using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.UserBook.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
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

        public async Task<Response<UserBookResponse>> CreateAsync(UserBook entity)
        {
            var userBookExisting = await _context.UserBooks
                .FirstOrDefaultAsync(us => us.UserId == entity.UserId && us.BookId == entity.BookId);

            if (userBookExisting != null)
                return Response<UserBookResponse>.Failure("The userBook Is already Exist", 409);

            await _repository.CreateAsync(entity);
            await _cache.RemoveAsync(GetAllCacheKey());
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);
            return Response<UserBookResponse>.Success(response);
        }

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

        public async Task<Response<UserBookResponse>> UpdateAsync(Guid Id, UserBook newEntity)
        {
            var userBookExisting = await _context.UserBooks.FindAsync(Id);
            if (userBookExisting == null)
                return Response<UserBookResponse>.Failure("The userBook Is Not Found", 404);

            userBookExisting.Statuts = newEntity.Statuts;
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
