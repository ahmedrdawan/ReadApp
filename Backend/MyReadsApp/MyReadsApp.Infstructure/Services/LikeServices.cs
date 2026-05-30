using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Like.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Infrastructure service for managing likes. Provides counting, adding, and removing likes,
    /// interacts with the data context and caching layer.
    /// </summary>
    public class LikeServices : ILikeServices
    {
        private readonly IGenericRepository<Like> _likeRepository;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public LikeServices(IGenericRepository<Like> likeRepository, AppDbContext context, IDistributedCache cache)
        {
            _likeRepository = likeRepository;
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Counts the total number of likes for a specific post, using cache when available.
        /// </summary>
        /// <param name="postId">The unique identifier of the post.</param>
        /// <returns>A Response containing the count of likes.</returns>
        public async Task<Response<int>> CountLikeAsync(Guid postId)
        {
            var cacheKey = GetCountCacheKey(postId);
            var cached = await _cache.GetRecordAsync<int?>(cacheKey);
            if (cached.HasValue)
                return cached.Value == 0
                    ? Response<int>.Failure("No Like For This Post", 404)
                    : Response<int>.Success(cached.Value);

            var likes = await _context.Likes
                .CountAsync(l=>l.PostId == postId);

            await _cache.SetRecordAsync(cacheKey, likes);

            if (likes == 0)
                return Response<int>.Failure("No Like For This Post", 404);

            return Response<int>.Success(likes);
        }

        /// <summary>
        /// Creates a new like after validating the user hasn't already liked the post.
        /// </summary>
        /// <param name="like">The like entity to create.</param>
        /// <returns>A Response containing the created like response.</returns>
        public async Task<Response<LikeResponse>> CreateAsync(Like like)
        {
            var likeExisting = await _context.Likes
                .SingleOrDefaultAsync(l => l.UserId == like.UserId && l.PostId == like.PostId);

            if (likeExisting != null)
                return Response<LikeResponse>.Failure("The User Already Like This Post", 409);
            await _likeRepository.CreateAsync(like);
            await _cache.RemoveAsync(GetCountCacheKey(like.PostId));
            return Response<LikeResponse>.Success(BuildResponse(like));
        }

        /// <summary>
        /// Removes a like from a post by a specific user.
        /// </summary>
        /// <param name="postId">The unique identifier of the post.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A Response containing the deleted like response.</returns>
        public async Task<Response<LikeResponse>> DeleteAsync(Guid postId, Guid userId)
        {
            var likeExisting = await _context.Likes
                .SingleOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);

            if (likeExisting == null)
                return Response<LikeResponse>.Failure("The User Don't Like This Post", 404);
            await _likeRepository.DeleteAsync(likeExisting);
            await _cache.RemoveAsync(GetCountCacheKey(postId));

            return Response<LikeResponse>.Success(BuildResponse(likeExisting));
        }

        private static string GetCountCacheKey(Guid postId) => $"PostLikesCount:{postId}";

        private static LikeResponse BuildResponse(Like like)
        {
            return new LikeResponse
            {
                PostId = like.PostId,
                UserId = like.UserId,
                CreatedAt = like.CreatedAt,
            };
        }
    }
}
