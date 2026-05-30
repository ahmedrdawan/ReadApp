using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.FriendShip;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Enums;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Services;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles friendship management endpoints including adding/removing friends and retrieving friend lists.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendShipController : ControllerBase
    {
        private readonly IFriendshipServices _friendshipServices;
        private readonly IUserAuthServices _userAuthServices;
        private readonly IRecommendionServices _recommendionServices;

        public FriendShipController(IFriendshipServices friendshipServices, IUserAuthServices userAuthServices, IRecommendionServices recommendionServices)
        {
            _friendshipServices = friendshipServices;
            _userAuthServices = userAuthServices;
            _recommendionServices = recommendionServices;
        }

        /// <summary>
        /// Adds a user as a friend.
        /// </summary>
        /// <param name="friendId">The unique identifier of the user to add as a friend.</param>
        /// <param name="request">Friendship creation data including status.</param>
        /// <returns>
        /// HTTP response indicating success or failure of adding friend.
        /// </returns>
        [HttpPost("{friendId}")]
        public async Task<IActionResult> AddFriend(Guid friendId, CreateFriendShipRequest request)
        {
            // Map API DTO to Core DTO (include current user and friend id)
            var coreRequest = new MyReadsApp.Core.DTOs.FriendShip.CreateFriendShipRequest(_userAuthServices.GetCurrentUser(), friendId, request.Status);
            var result = await _friendshipServices.CreateAsync(coreRequest);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Removes a user from the friend list.
        /// </summary>
        /// <param name="friendId">The unique identifier of the friend to remove.</param>
        /// <returns>
        /// HTTP response indicating success or failure of removing friend.
        /// </returns>
        [HttpDelete("{friendId}")]
        public async Task<IActionResult> DeleteFriend(Guid friendId)
        {

            var result = await _friendshipServices.DeleteAsync(_userAuthServices.GetCurrentUser(), friendId);
            return StatusCode(result.StatusCode, result);

        }

        /// <summary>
        /// Retrieves suggested friends based on user preferences.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of friend suggestions.
        /// </returns>
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions()
        {
            var result = await _recommendionServices.FriendsSuggestionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all accepted friends of the current user.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of accepted friends.
        /// </returns>
        [HttpGet("accepted")]
        public async Task<IActionResult> GetFriends()
        {
            var result = await _friendshipServices
                .GetAllAsync((fh) => fh.UserId == _userAuthServices.GetCurrentUser() && fh.Status == FriendShipStatus.accepted);
            
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all blocked friends of the current user.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of blocked friends.
        /// </returns>
        [HttpGet("bloked")]
        public async Task<IActionResult> GetBlokedFriends()
        {
            var result = await _friendshipServices
                .GetAllAsync((fh) => fh.UserId == _userAuthServices.GetCurrentUser() && fh.Status == FriendShipStatus.blocked);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all pending friend requests for the current user.
        /// </summary>
        /// <returns>
        /// HTTP response containing collection of pending friend requests.
        /// </returns>
        [HttpGet("pending")]
        public async Task<IActionResult> GetpendingFriends()
        {
            var result = await _friendshipServices
                .GetAllAsync((fh) => fh.UserId == _userAuthServices.GetCurrentUser() && fh.Status == FriendShipStatus.pending);

            return Ok(result);
        }
    }
}
