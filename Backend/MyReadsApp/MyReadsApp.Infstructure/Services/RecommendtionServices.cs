
using MyReadsApp.Core.DTOs.FriendShip.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyReadsApp.Core.Enums;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.Infstructure.Services
{
    public class RecommendtionServices : IRecommendionServices
    {
        private readonly IFriendshipServices _friendshipServices;
        private readonly IUserAuthServices _userAuthServices;

        public RecommendtionServices(IFriendshipServices friendshipServices, IUserAuthServices userAuthServices)
        {
            _friendshipServices = friendshipServices;
            _userAuthServices = userAuthServices;
        }

        public async Task<Response<List<FriendResponse>>> FriendsSuggestionAsync()
        {
            var userId = _userAuthServices.GetCurrentUser();
            Queue<Guid> queue = new Queue<Guid>();
            HashSet<Guid> visited = new HashSet<Guid>();
            HashSet<Guid> existingFriends = new HashSet<Guid>();

            Dictionary<Guid, int> mutualFriends = new Dictionary<Guid, int>();

            var userFriends = await _friendshipServices.GetAllAsync(f => f.UserId == userId);

            foreach (var friend in userFriends)
            {
                existingFriends.Add(friend.UserFriendId);
                queue.Enqueue(friend.UserFriendId);
                visited.Add(friend.UserFriendId);
            }

            int level = 1;

            while (queue.Count > 0 && level <= 2)
            {
                int size = queue.Count;

                for (int i = 0; i < size; i++)
                {
                    var current = queue.Dequeue();

                    var friends = await _friendshipServices.GetAllAsync(
                        f => f.UserId == current
                    );

                    foreach (var f in friends)
                    {
                        if (f.UserFriendId == userId)
                            continue;

                        if (level == 2)
                        {
                            if (!existingFriends.Contains(f.UserFriendId))
                            {
                                if (!mutualFriends.ContainsKey(f.UserFriendId))
                                    mutualFriends[f.UserFriendId] = 0;

                                mutualFriends[f.UserFriendId]++;
                            }
                        }

                        if (!visited.Contains(f.UserFriendId))
                        {
                            visited.Add(f.UserFriendId);
                            queue.Enqueue(f.UserFriendId);
                        }
                    }
                }

                level++;
            }

            var ordered = mutualFriends
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .ToList();

            List<FriendResponse> suggestions = new List<FriendResponse>();

            foreach (var id in ordered)
            {
                suggestions.Add(new FriendResponse
                {
                    UserFriendId = id,
                    Status = FriendShipStatus.pending
                });
            }

            return Response<List<FriendResponse>>.Success(suggestions);
        }
    }
}
