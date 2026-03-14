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

        public async Task<Response<AuthorResponse>> CreateAsync(Author entity)
        {
            var exists = await _context.Authors
                .AnyAsync(a => a.AuthorName == entity.AuthorName);

            if (exists)
                return Response<AuthorResponse>.Failure("The Author Already Exists", 409);

            await _genericRepository.CreateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);

            return Response<AuthorResponse>.Success(response);
        }

        public async Task<Response<AuthorResponse>> DeleteAsync(Guid authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
                return Response<AuthorResponse>.Failure("The Author Not Found",404);

            await _genericRepository.DeleteAsync(author);
            await _cache.RemoveAsync(GetCacheKey(authorId));

            return Response<AuthorResponse>.Success(BuildResponse(author));
        }

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

        public async Task<Response<AuthorResponse>> UpdateAsync(Guid authorId, Author newEntity)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
                return Response<AuthorResponse>.Failure("The Author Not Found", 404);

            if (!string.IsNullOrEmpty(newEntity.Bio))
                author.Bio = newEntity.Bio;

            if (!string.IsNullOrEmpty(newEntity.AuthorName))
                author.AuthorName = newEntity.AuthorName;

            if (!string.IsNullOrEmpty(newEntity.AuthorImage))
                author.AuthorImage = newEntity.AuthorImage;

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
