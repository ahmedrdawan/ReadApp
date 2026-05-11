using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    public class BookServices : IBookServices
    {
        private readonly IGenericRepository<Book> _genericRepository;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

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

            var response = BuildResponse(entity, author?.AuthorName);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);

            return Response<BookAuthorResponse>.Success(response);
        }

        public async Task<Response<BookAuthorResponse>> DeleteAsync(Guid BookId)
        {
            var book = await _context.Books.FindAsync(BookId);
            if (book == null)
                return Response<BookAuthorResponse>.Failure("The Book Not Found", 404);

            await _genericRepository.DeleteAsync(book);
            await _cache.RemoveAsync(GetCacheKey(BookId));
            var author = await _context.Authors.FindAsync(book.AuthorId);
            return Response<BookAuthorResponse>.Success(BuildResponse(book, author?.AuthorName));
        }

        public async Task<Response<BookAuthorResponse>> GetAsync(Guid BookId)
        {
            string cacheKey = GetCacheKey(BookId);
            var cached = await _cache.GetRecordAsync<BookAuthorResponse>(cacheKey);
            if (cached is not null)
                return Response<BookAuthorResponse>.Success(cached);

            var book = await _context.Books.FindAsync(BookId);
            if (book == null)
                return Response<BookAuthorResponse>.Failure("The Book Not Found", 404);

            var author = await _context.Authors.FindAsync(book.AuthorId);
            var response = BuildResponse(book, author?.AuthorName);
            await _cache.SetRecordAsync(cacheKey, response);
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
            var author = await _context.Authors.FindAsync(entity.AuthorId);
            var response = BuildResponse(entity, author?.AuthorName);
            await _cache.SetRecordAsync(GetCacheKey(id), response);
            return Response<BookAuthorResponse>.Success(response);
        }

        private static string GetCacheKey(Guid id) => $"Book:{id}";

        public async Task<Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>> GetListAsync(int pageNumber = 1, int pageSize = 10, Guid? categoryId = null)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Books.AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(b => _context.BookCategories.Any(bc => bc.BookId == b.Id && bc.CategoryId == categoryId.Value));
            }

            var total = await query.LongCountAsync();
            var books = await query.OrderByDescending(b => b.Title).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var items = new List<BookAuthorResponse>();
            foreach (var book in books)
            {
                var author = await _context.Authors.FindAsync(book.AuthorId);
                items.Add(BuildResponse(book, author?.AuthorName));
            }

            var paged = new MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            };

            return Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>.Success(paged);
        }

        public async Task<Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>> SearchAsync(string query, int pageNumber = 1, int pageSize = 10, Guid? categoryId = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetListAsync(pageNumber, pageSize, categoryId);

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var q = _context.Books.Where(b => b.Title.Contains(query) || (b.Description != null && b.Description.Contains(query)));
            if (categoryId.HasValue)
                q = q.Where(b => _context.BookCategories.Any(bc => bc.BookId == b.Id && bc.CategoryId == categoryId.Value));

            var total = await q.LongCountAsync();
            var books = await q.OrderByDescending(b => b.Title).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var items = new List<BookAuthorResponse>();
            foreach (var book in books)
            {
                var author = await _context.Authors.FindAsync(book.AuthorId);
                items.Add(BuildResponse(book, author?.AuthorName));
            }

            var paged = new MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            };

            return Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>.Success(paged);
        }

        public async Task<Response<object>> RateBookAsync(Guid bookId, Guid userId, int value)
        {
            if (value < 1 || value > 5)
                return Response<object>.Failure("Rating value must be between 1 and 5", 400);

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return Response<object>.Failure("The Book Not Found", 404);

            var existing = await _context.BookRatings.FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
            if (existing != null)
            {
                existing.Value = value;
                await _context.SaveChangesAsync();
            }
            else
            {
                var rating = new BookRating { Id = Guid.NewGuid(), BookId = bookId, UserId = userId, Value = value, CreatedAt = DateTime.UtcNow };
                await _context.BookRatings.AddAsync(rating);
                await _context.SaveChangesAsync();
            }

            return Response<object>.Success(null, 200);
        }

        public async Task<Response<object>> GetRatingSummaryAsync(Guid bookId, Guid? userId = null)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return Response<object>.Failure("The Book Not Found", 404);

            var ratings = _context.BookRatings.Where(r => r.BookId == bookId);
            var total = await ratings.LongCountAsync();
            var average = total == 0 ? 0 : await ratings.AverageAsync(r => r.Value);
            int? userValue = null;
            if (userId.HasValue)
            {
                var u = await ratings.FirstOrDefaultAsync(r => r.UserId == userId.Value);
                if (u != null) userValue = u.Value;
            }

            var resp = new { average, count = total, userValue };
            return Response<object>.Success(resp);
        }

        private static BookAuthorResponse BuildResponse(Book entity, string? authorName)
        {
            return new BookAuthorResponse
            {
                Id = entity.Id,
                BookImage = entity.BookImage,
                Description = entity.Description,
                AuthorId = entity.AuthorId,
                Title = entity.Title,
                AuthorName = authorName
            };
        }
    }
}
