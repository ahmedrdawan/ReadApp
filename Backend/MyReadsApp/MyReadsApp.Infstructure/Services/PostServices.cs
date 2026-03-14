using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Post.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    public class PostServices : IPostServices
    {
        private readonly IGenericRepository<Post> _genericRepository;
        private readonly IUserAuthServices _userAuthServices;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public PostServices(IGenericRepository<Post> genericRepository, AppDbContext context, IUserAuthServices userAuthServices, IDistributedCache cache)
        {
            _genericRepository = genericRepository;
            _context = context;
            _userAuthServices = userAuthServices;
            _cache = cache;
        }

        public async Task<Response<PostResponse>> CreateAsync(Post entity)
        {
            var book = await _context.Books.FindAsync(entity.BookId);
            var user = await _context.Users.FindAsync(entity.UserId);

            if (book is null || user is null)
                return Response<PostResponse>.Failure("The Book Or User Not Found", 404);

            await _genericRepository.CreateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);
            return Response<PostResponse>.Success(response);
        }

        public async Task<Response<PostResponse>> DeleteAsync(Guid PostId)
        {
            var post = await _context.Posts.FindAsync(PostId);
            if (post is null)
                return Response<PostResponse>.Failure("The Post Not Found",404);

            if (post.UserId != _userAuthServices.GetCurrentUser())
                return Response<PostResponse>.Failure("The User Not Authorize", 403);

            await _genericRepository.DeleteAsync(post);
            await _cache.RemoveAsync(GetCacheKey(PostId));
            return Response<PostResponse>.Success(BuildResponse(post));
        }

        public async Task<Response<PostResponse>> GetAsync(Guid PostId)
        {
            var cached = await _cache.GetRecordAsync<PostResponse>(GetCacheKey(PostId));
            if (cached is not null)
                return Response<PostResponse>.Success(cached);

            var post = await _context.Posts.FindAsync(PostId);
            if (post == null)
                return Response<PostResponse>.Failure("The Post Not Found", 404);

            var response = BuildResponse(post);
            await _cache.SetRecordAsync(GetCacheKey(PostId), response);
            return Response<PostResponse>.Success(response);
        }

        public async Task<Response<PostResponse>> UpdateAsync(Guid PostId, Post NewEntity)
        {
            var post = await _context.Posts.FindAsync(PostId);
            var book = await _context.Books.FindAsync(NewEntity.BookId);
            if (post is null || book is null)
                return Response<PostResponse>.Failure($"Post or Book not found.", 404);

            if (post.UserId != _userAuthServices.GetCurrentUser())
                return Response<PostResponse>.Failure("The User Not Authorize", 403);


            post.BookId = NewEntity.BookId;
            post.UserId = NewEntity.UserId;
            post.UpdatedAt = NewEntity.UpdatedAt;

            await _genericRepository.UpdateAsync(post);
            var response = BuildResponse(post);
            await _cache.SetRecordAsync(GetCacheKey(PostId), response);
            return Response<PostResponse>.Success(response);
        }

        private static string GetCacheKey(Guid id) => $"Post:{id}";

        private static PostResponse BuildResponse(Post entity)
        {
            return new PostResponse
            {
                Id = entity.Id,
                BookId = entity.BookId,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
