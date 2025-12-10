using MyReadsApp.Core.DTOs.Author.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;

namespace MyReadsApp.Infstructure.Services
{
    public class AuthorServices : IAuthorServices
    {
        private readonly IGenericRepository<Author> _genericRepository;
        private readonly AppDbContext _context;

        public AuthorServices(IGenericRepository<Author> genericRepository, AppDbContext context)
        {
            _genericRepository = genericRepository;
            _context = context;
        }

        public async Task<Response<AuthorResponse>> CreateAsync(Author entity)
        {
            var exists = await _context.Authors
                .AnyAsync(a => a.AuthorName == entity.AuthorName);

            if (exists)
                return Response<AuthorResponse>.Failure("The Author Already Exists", 409);

            await _genericRepository.CreateAsync(entity);

            return Response<AuthorResponse>.Success(BuildResponse(entity));
        }

        public async Task<Response<AuthorResponse>> DeleteAsync(Guid authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
                return Response<AuthorResponse>.Failure("The Author Not Found",404);

            await _genericRepository.DeleteAsync(authorId);

            return Response<AuthorResponse>.Success(BuildResponse(author));
        }

        public async Task<Response<AuthorResponse>> GetAsync(Guid authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);

            return author == null
                ? Response<AuthorResponse>.Failure("The Author Not Found", 404)
                : Response<AuthorResponse>.Success(BuildResponse(author));
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

            await _genericRepository.UpdateAsync(authorId, author);

            return Response<AuthorResponse>.Success(BuildResponse(author));
        }

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
