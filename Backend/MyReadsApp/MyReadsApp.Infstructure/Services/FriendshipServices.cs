using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FriendShip.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using System.Linq.Expressions;

namespace MyReadsApp.Infstructure.Services
{
    public class FriendshipServices : IFriendshipServices
    {
        private readonly IGenericRepository<FriendShip> _repository;
        private readonly AppDbContext _context;

        public FriendshipServices(IGenericRepository<FriendShip> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<Response<FriendShipResponse>> CreateAsync(FriendShip entity)
        {
            var friendShipExisting = await _context.FriendShips
                .FirstOrDefaultAsync(fh => fh.UserId == entity.UserId && fh.FriendId == entity.FriendId);

            if (friendShipExisting != null)
                return Response<FriendShipResponse>.Failure("The Friend Ship Is already Exist", 409);

            await _repository.CreateAsync(entity);

            return Response<FriendShipResponse>.Success(await BuildRepsonse(entity));
        }

        

        public async Task<Response<FriendShipResponse>> DeleteAsync(Guid SendUserId, Guid ReceivedUserId)
        {
            var friendShipExisting = await _context.FriendShips
                .FirstOrDefaultAsync(fh => fh.UserId == SendUserId && fh.FriendId == ReceivedUserId);


            if (friendShipExisting == null)
                return Response<FriendShipResponse>.Failure("TheFriend Ship Not Found", 404);

            await _repository.DeleteAsync(friendShipExisting);
            return Response<FriendShipResponse>.Success(await BuildRepsonse(friendShipExisting));

        }

        public async Task<IEnumerable<FriendResponse>> GetAllAsync(Expression<Func<FriendShip, bool>> filter)
        {
            return await _context.FriendShips.Where(filter)
                .Select(fh => new FriendResponse
                {
                    UserFriend = fh.FriendUser.UserName,
                    CreatedAt = fh.CreatedAt,
                    Status = fh.Status
                }).ToListAsync();
        }

        private async Task<FriendShipResponse> BuildRepsonse(FriendShip entity)
        {
            return await _context.FriendShips
                .Where(fh => fh.UserId == entity.UserId && fh.FriendId == entity.FriendId)
                .Select(fh => new FriendShipResponse
                {
                    SendUserName = fh.User.UserName,
                    ReceivedUserName = fh.FriendUser.UserName,
                    CreatedAt = fh.CreatedAt,
                    Status = fh.Status,
                })
                .FirstOrDefaultAsync();
        }
    }
}
