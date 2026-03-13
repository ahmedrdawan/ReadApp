using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MyReadsApp.Infstructure.Services
{
    public class BookServices : IBookServices
    {
        private readonly IGenericRepository<Book> _genericRepository;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
        public BookServices(IGenericRepository<Book> genericRepository, AppDbContext context, IDistributedCache cache)
        {
            _genericRepository = genericRepository;
            _context = context;
            _cache = cache;
        }

        public async Task<Response<BookAuthorResponse>> CreateAsync(Book entity)
        {
            var author = await _context.Authors.FindAsync(entity.AuthorId);
            if (author == null)
                return Response<BookAuthorResponse>.Failure("The Author Not Found", 404);

            var exists = await _context.Books
                .AnyAsync(x => x.Title == entity.Title && x.AuthorId == entity.AuthorId);

            if (exists)
                return Response<BookAuthorResponse>.Failure("The Book Already Exist", 409);
            await _genericRepository.CreateAsync(entity);

            var response = BuildResponse(entity);
            await SetBookInCacheAsync(entity.Id, response);

            return Response<BookAuthorResponse>.Success(response);
        }

        public async Task<Response<BookAuthorResponse>> DeleteAsync(Guid BookId)
        {
            var book = await _context.Books.FindAsync(BookId);
            if (book == null)
                return Response<BookAuthorResponse>.Failure("The Book Not Found", 404);

            await _genericRepository.DeleteAsync(book);
            await _cache.RemoveAsync(GetCacheKey(BookId));
            return Response<BookAuthorResponse>.Success(BuildResponse(book));
        }
        

        public async Task<Response<BookAuthorResponse>> GetAsync(Guid BookId)
        {
            string cacheKey = GetCacheKey(BookId);
            string? cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                var cachedResponse = JsonSerializer.Deserialize<BookAuthorResponse>(cachedData, _jsonOptions);
                if (cachedResponse != null)
                    return Response<BookAuthorResponse>.Success(cachedResponse);
            }

            var book = await _context.Books.FindAsync(BookId);
            if (book == null)
                return Response<BookAuthorResponse>.Failure("The Book Not Found", 404);

            var response = BuildResponse(book);
            await SetBookInCacheAsync(book.Id, response);
            return Response<BookAuthorResponse>.Success(response);
        }

        public async Task<Response<BookAuthorResponse>> UpdateAsync(Guid id, Book newEntity)
        {
            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
                return Response<BookAuthorResponse>.Failure($"Book with Id '{id}' not found.", 404);

            var authorExists = await _context.Authors.AnyAsync(a => a.Id == newEntity.AuthorId);
            if (!authorExists)
                return Response<BookAuthorResponse>.Failure($"Author with Id '{newEntity.AuthorId}' not found.", 404);

            if (!string.IsNullOrEmpty(newEntity.BookImage))
                entity.BookImage = newEntity.BookImage;
            if (!string.IsNullOrEmpty(newEntity.Description))
                entity.Description = newEntity.Description;
            if (!string.IsNullOrEmpty(newEntity.Title))
                entity.Title = newEntity.Title;

            entity.AuthorId = newEntity.AuthorId;

            await _genericRepository.UpdateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.RemoveAsync(GetCacheKey(id));
            await SetBookInCacheAsync(entity.Id, response);
            return Response<BookAuthorResponse>.Success(response);
        }

        #region Cache Methods
        private static string GetCacheKey(Guid id) => $"Book:{id}";
        private async Task SetBookInCacheAsync(Guid Id, BookAuthorResponse response)
        {
            string value = JsonSerializer.Serialize(response, _jsonOptions);
            await _cache.SetStringAsync(GetCacheKey(Id), value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            });
        }
        #endregion

        #region Helper Methods
        private static BookAuthorResponse BuildResponse(Book entity)
        {
            return new BookAuthorResponse
            {
                Id = entity.Id,
                BookImage = entity.BookImage,
                Description = entity.Description,
                AuthorId = entity.AuthorId,
                Title = entity.Title
            };
        }
        #endregion
    }
}
