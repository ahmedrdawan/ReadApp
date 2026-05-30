using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FriendShip.Response;
using MyReadsApp.Core.DTOs.FriendShip.Request;
using MyReadsApp.Core.Entities;
using System.Linq.Expressions;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IFriendshipServices
    {
        Task<Response<FriendShipResponse>> CreateAsync(CreateFriendShipRequest request);
        Task<Response<FriendShipResponse>> DeleteAsync(Guid SendUserId, Guid ReceivedUserId);
        Task<IEnumerable<FriendResponse>> GetAllAsync(Expression<Func<FriendShip, bool>> filter);
    }
}
