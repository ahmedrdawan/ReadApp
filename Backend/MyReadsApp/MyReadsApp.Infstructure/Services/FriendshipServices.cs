using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FriendShip.Response;
using MyReadsApp.Core.DTOs.FriendShip.Request;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using System.Linq.Expressions;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Manages friendship relations between users in the infrastructure layer. Handles creating,
    /// listing and removing friendships and updates persistence and cache as needed.
    /// </summary>
    public class FriendshipServices : IFriendshipServices
    {
        private readonly IGenericRepository<FriendShip> _repository;
        private readonly AppDbContext _context;

        public FriendshipServices(IGenericRepository<FriendShip> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Creates a new friendship after validating uniqueness.
        /// </summary>
        /// <param name="request">Create friendship request DTO.</param>
        /// <returns>A Response containing the created friendship response.</returns>
        public async Task<Response<FriendShipResponse>> CreateAsync(CreateFriendShipRequest request)
        {
            var friendShipExisting = await _context.FriendShips
                .FirstOrDefaultAsync(fh => fh.UserId == request.UserId && fh.FriendId == request.FriendId
                || fh.UserId == request.FriendId && fh.FriendId == request.UserId);

            if (friendShipExisting != null)
                return Response<FriendShipResponse>.Failure("The Friend Ship Is already Exist", 409);

            var entity = new FriendShip
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                FriendId = request.FriendId,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(entity);

            return Response<FriendShipResponse>.Success(await BuildRepsonse(entity));
        }

        

        /// <summary>
        /// Deletes a friendship between two users.
        /// </summary>
        /// <param name="SendUserId">The identifier of the user who initiated the friendship.</param>
        /// <param name="ReceivedUserId">The identifier of the friend.</param>
        /// <returns>A Response containing the deleted friendship response.</returns>
        public async Task<Response<FriendShipResponse>> DeleteAsync(Guid SendUserId, Guid ReceivedUserId)
        {
            var friendShipExisting = await _context.FriendShips
                .FirstOrDefaultAsync(fh => fh.UserId == SendUserId && fh.FriendId == ReceivedUserId
                || fh.UserId == SendUserId && fh.FriendId == ReceivedUserId);


            if (friendShipExisting == null)
                return Response<FriendShipResponse>.Failure("TheFriend Ship Not Found", 404);

            await _repository.DeleteAsync(friendShipExisting);
            return Response<FriendShipResponse>.Success(await BuildRepsonse(friendShipExisting));

        }

        /// <summary>
        /// Retrieves all friendships matching the provided filter criteria.
        /// </summary>
        /// <param name="filter">LINQ expression to filter friendships.</param>
        /// <returns>An enumerable collection of friend responses matching the filter.</returns>
        public async Task<IEnumerable<FriendResponse>> GetAllAsync(Expression<Func<FriendShip, bool>> filter)
        {
            return await _context.FriendShips
                .AsNoTracking()
                .Where(filter)
                .Select(fh => new FriendResponse
                {
                    UserFriendId = fh.FriendId,
                    CreatedAt = fh.CreatedAt,
                    Status = fh.Status
                }).ToListAsync();
        }

        #region Helper Method
        private async Task<FriendShipResponse> BuildRepsonse(FriendShip entity)
        {
            var friendship =  await _context.FriendShips
                .Where(fh => fh.UserId == entity.UserId && fh.FriendId == entity.FriendId)
                .Select(fh => new FriendShipResponse
                {
                    SendUserName = fh.User.UserName,
                    ReceivedUserName = fh.FriendUser.UserName,
                    CreatedAt = fh.CreatedAt,
                    Status = fh.Status,
                })
                .FirstOrDefaultAsync();
            if (friendship == null)
                return new FriendShipResponse();

            return friendship;
        }
        #endregion
    }
}
