using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.UserFollow.Response;
using MyReadsApp.Core.Entities;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IUserfollowServices
    {
        Task<Response<UserFollowResponse>> CreateAsync(UserFollow entity);
        Task<Response<UserFollowResponse>> DeleteAsync(Guid SendUserId, Guid ReceivedUserId);

        Task<IEnumerable<UserfollowersResponse>> GetFollowersAsync(Guid userId);
        Task<IEnumerable<UserfollowingsResponse>> GetFollowingsAsync(Guid userId);
    }
}
