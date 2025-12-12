using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.UserFollow.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;

namespace MyReadsApp.Infstructure.Services
{
    public class UserfollowServices : IUserfollowServices
    {
        private readonly IGenericRepository<UserFollow> _repository;
        private readonly AppDbContext _context;

        public UserfollowServices(IGenericRepository<UserFollow> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<Response<UserFollowResponse>> CreateAsync(UserFollow entity)
        {
            var userFollowExisting = await _context.UserFollows
                .SingleOrDefaultAsync(uf => uf.FollowerId == entity.FollowerId && uf.FollowingId == entity.FollowingId);

            if (userFollowExisting != null)
                return Response<UserFollowResponse>.Failure("The User Is Follwoing ", 409);

            await _repository.CreateAsync(entity);
            return Response<UserFollowResponse>.Success(await BuildResponse(entity));
        }

        public async Task<Response<UserFollowResponse>> DeleteAsync(Guid SendUserId, Guid ReceivedUserId)
        {
            var userFollowExisting = await _context.UserFollows
                .SingleOrDefaultAsync(uf => uf.FollowerId == SendUserId && uf.FollowingId == ReceivedUserId);

            if (userFollowExisting == null)
                return Response<UserFollowResponse>.Failure("The User Not Follwoing ", 404);

            await _repository.DeleteAsync(userFollowExisting);
            return Response<UserFollowResponse>.Success(await BuildResponse(userFollowExisting));

        }

        public async Task<IEnumerable<UserfollowersResponse>> GetFollowersAsync(Guid userId)
        {
            var followers = await _context.UserFollows
                .Where(uf => uf.FollowingId == userId)
                .Select(uf => new UserfollowersResponse
                {
                    UserNameFollower = uf.UserFollowers.UserName,
                    CreatedAt = uf.CreatedAt
                })
                .ToListAsync();


            return followers;
        }

        public async Task<IEnumerable<UserfollowingsResponse>> GetFollowingsAsync(Guid userId)
        {
            var followings = await _context.UserFollows
                .Where(uf => uf.FollowerId == userId)
                .Select(uf => new UserfollowingsResponse
                {
                    UserNameFollowing = uf.UserFollowing.UserName,
                    CreatedAt = uf.CreatedAt
                })
                .ToListAsync();


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
    }
}
