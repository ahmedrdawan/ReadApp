using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.UserFollow.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Handles user follow/unfollow operations and related queries in the infrastructure layer.
    /// Interacts with repositories and maintains relevant caches.
    /// </summary>
    public class UserfollowServices : IUserfollowServices
    {
        private readonly IGenericRepository<UserFollow> _repository;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public UserfollowServices(IGenericRepository<UserFollow> repository, AppDbContext context, IDistributedCache cache)
        {
            _repository = repository;
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Creates a new user follow relationship after validating uniqueness.
        /// </summary>
        /// <param name="entity">The user follow entity to create.</param>
        /// <returns>A Response containing the created user follow response.</returns>
        public async Task<Response<UserFollowResponse>> CreateAsync(UserFollow entity)
        {
            var userFollowExisting = await _context.UserFollows
                .SingleOrDefaultAsync(uf => uf.FollowerId == entity.FollowerId && uf.FollowingId == entity.FollowingId);

            if (userFollowExisting != null)
                return Response<UserFollowResponse>.Failure("The User Is Follwoing ", 409);

            await _repository.CreateAsync(entity);
            await InvalidateFollowCaches(entity.FollowerId, entity.FollowingId);
            return Response<UserFollowResponse>.Success(await BuildResponse(entity));
        }

        /// <summary>
        /// Deletes a user follow relationship between two users.
        /// </summary>
        /// <param name="SendUserId">The identifier of the user who is following.</param>
        /// <param name="ReceivedUserId">The identifier of the user being followed.</param>
        /// <returns>A Response containing the deleted user follow response.</returns>
        public async Task<Response<UserFollowResponse>> DeleteAsync(Guid SendUserId, Guid ReceivedUserId)
        {
            var userFollowExisting = await _context.UserFollows
                .SingleOrDefaultAsync(uf => uf.FollowerId == SendUserId && uf.FollowingId == ReceivedUserId);

            if (userFollowExisting == null)
                return Response<UserFollowResponse>.Failure("The User Not Follwoing ", 404);

            await _repository.DeleteAsync(userFollowExisting);
            await InvalidateFollowCaches(SendUserId, ReceivedUserId);
            return Response<UserFollowResponse>.Success(await BuildResponse(userFollowExisting));

        }

        /// <summary>
        /// Retrieves all followers of a specific user, using cache when available.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>An enumerable collection of user followers responses.</returns>
        public async Task<IEnumerable<UserfollowersResponse>> GetFollowersAsync(Guid userId)
        {
            var cacheKey = $"UserFollow:Followers:{userId}";
            var cached = await _cache.GetRecordAsync<List<UserfollowersResponse>>(cacheKey);
            if (cached is not null)
                return cached;

            var followers = await _context.UserFollows
                .Where(uf => uf.FollowingId == userId)
                .Select(uf => new UserfollowersResponse
                {
                    UserNameFollower = uf.UserFollowers.UserName,
                    CreatedAt = uf.CreatedAt
                })
                .ToListAsync();

            await _cache.SetRecordAsync(cacheKey, followers);
            return followers;
        }

        /// <summary>
        /// Retrieves all users being followed by a specific user, using cache when available.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>An enumerable collection of user followings responses.</returns>
        public async Task<IEnumerable<UserfollowingsResponse>> GetFollowingsAsync(Guid userId)
        {
            var cacheKey = $"UserFollow:Followings:{userId}";
            var cached = await _cache.GetRecordAsync<List<UserfollowingsResponse>>(cacheKey);
            if (cached is not null)
                return cached;

            var followings = await _context.UserFollows
                .Where(uf => uf.FollowerId == userId)
                .Select(uf => new UserfollowingsResponse
                {
                    UserNameFollowing = uf.UserFollowing.UserName,
                    CreatedAt = uf.CreatedAt
                })
                .ToListAsync();

            await _cache.SetRecordAsync(cacheKey, followings);
            return followings;
        }

        private async Task<UserFollowResponse> BuildResponse(UserFollow entity)
        {
            return await _context.UserFollows
                .Where(uf => uf.FollowerId == entity.FollowerId && uf.FollowingId == entity.FollowingId)
                .Select(uf => new UserFollowResponse
                {
                    SendUserFullName = uf.UserFollowers.UserName,
                    ReceiveUserFullName = uf.UserFollowing.UserName,
                    CreatedAt = uf.CreatedAt
                }).SingleOrDefaultAsync();
        }

        private async Task InvalidateFollowCaches(Guid followerId, Guid followingId)
        {
            await _cache.RemoveAsync($"UserFollow:Followers:{followingId}");
            await _cache.RemoveAsync($"UserFollow:Followings:{followerId}");
        }
    }
}
