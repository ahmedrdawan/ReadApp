using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FriendShip.Response;
using MyReadsApp.Core.Entities;
using System.Linq.Expressions;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IFriendshipServices
    {
        Task<Response<FriendShipResponse>> CreateAsync(FriendShip entity);
        Task<Response<FriendShipResponse>> DeleteAsync(Guid SendUserId, Guid ReceivedUserId);
        Task<IEnumerable<FriendResponse>> GetAllAsync(Expression<Func<FriendShip, bool>> filter);
    }
}
