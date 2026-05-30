using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Author.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Handles author domain persistence and retrieval in the infrastructure layer, including caching
    /// and transformation into response DTOs for API consumption.
    /// </summary>
    public class AuthorServices : IAuthorServices
    {
        private readonly IGenericRepository<Author> _genericRepository;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public AuthorServices(IGenericRepository<Author> genericRepository, AppDbContext context, IDistributedCache cache)
        {
            _genericRepository = genericRepository;
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Creates a new author in the database after validating uniqueness.
        /// </summary>
        /// <param name="request">Create author request DTO.</param>
        /// <returns>A Response containing the created author response.</returns>
        public async Task<Response<AuthorResponse>> CreateAsync(CreateAuthorRequest request)
        {
            var exists = await _context.Authors
                .AnyAsync(a => a.AuthorName == request.AuthorName);

            if (exists)
                return Response<AuthorResponse>.Failure("The Author Already Exists", 409);

            var entity = new Author
            {
                Id = Guid.NewGuid(),
                AuthorName = request.AuthorName,
                Bio = request.Bio,
                AuthorImage = request.AuthorImage,
                CreatedAt = DateTime.UtcNow
            };

            await _genericRepository.CreateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);

            return Response<AuthorResponse>.Success(response);
        }

        /// <summary>
        /// Deletes an author by its identifier and removes associated cache entry.
        /// </summary>
        /// <param name="authorId">The unique identifier of the author to delete.</param>
        /// <returns>A Response containing the deleted author response.</returns>
        public async Task<Response<AuthorResponse>> DeleteAsync(Guid authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
                return Response<AuthorResponse>.Failure("The Author Not Found",404);

            await _genericRepository.DeleteAsync(author);
            await _cache.RemoveAsync(GetCacheKey(authorId));

            return Response<AuthorResponse>.Success(BuildResponse(author));
        }

        /// <summary>
        /// Retrieves an author by its identifier, using cache when available.
        /// </summary>
        /// <param name="authorId">The unique identifier of the author.</param>
        /// <returns>A Response containing the author response.</returns>
        public async Task<Response<AuthorResponse>> GetAsync(Guid authorId)
        {
            var cached = await _cache.GetRecordAsync<AuthorResponse>(GetCacheKey(authorId));
            if (cached is not null)
                return Response<AuthorResponse>.Success(cached);

            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
                return Response<AuthorResponse>.Failure("The Author Not Found", 404);

            var response = BuildResponse(author);
            await _cache.SetRecordAsync(GetCacheKey(authorId), response);
            return Response<AuthorResponse>.Success(response);
        }

        /// <summary>
        /// Updates an existing author with the provided request data.
        /// </summary>
        /// <param name="authorId">The unique identifier of the author to update.</param>
        /// <param name="request">Update author request DTO.</param>
        /// <returns>A Response containing the updated author response.</returns>
        public async Task<Response<AuthorResponse>> UpdateAsync(Guid authorId, UpdateAuthorRequest request)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
                return Response<AuthorResponse>.Failure("The Author Not Found", 404);

            if (!string.IsNullOrEmpty(request.Bio))
                author.Bio = request.Bio;

            if (!string.IsNullOrEmpty(request.AuthorName))
                author.AuthorName = request.AuthorName;

            if (!string.IsNullOrEmpty(request.AuthorImage))
                author.AuthorImage = request.AuthorImage;

            await _genericRepository.UpdateAsync(author);
            var response = BuildResponse(author);
            await _cache.SetRecordAsync(GetCacheKey(authorId), response);

            return Response<AuthorResponse>.Success(response);
        }

        private static string GetCacheKey(Guid id) => $"Author:{id}";

        private static AuthorResponse BuildResponse(Author author)
        {
            return new AuthorResponse
            {
                Id = author.Id,
                AuthorName = author.AuthorName,
                AuthorImage = author.AuthorImage,
                Bio = author.Bio,
            };
        }
    }

}
